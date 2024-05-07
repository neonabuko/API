using System.Text;
using ScoreHubAPI.Entities;
using ScoreHubAPI.Entities.Dto;
using ScoreHubAPI.Repositories;

namespace ScoreHubAPI.Service;

public class MusicService<T>(IMusicRepository<T> musicRepository, string _musicPath)
: IMusicService<T> where T : Music
{
    private readonly string[] allowedScoreExtensions = [".mei", ".musicxml"];

    public async Task<ICollection<T>> GetAllDataAsync()
    {
        var musics = await musicRepository.GetAllAsync();
        return musics;
    }

    public async Task<T> GetDataByNameAsync(string name)
    {
        var music = await musicRepository.GetByNameAsync(name);
        return music.GetValueOrThrow();
    }

    public async Task<FileStream> GetFileByNameAsync(string name)
    {
        return await Task.Run(() =>
        {
            var nameNoExtension = Path.GetFileNameWithoutExtension(name);
            string path = Path.Combine($"{_musicPath}/{nameNoExtension}", name);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found", path);
            }
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        });
    }


    public async Task<int> SaveDataAsync(T music)
    {
        var musicId = await musicRepository.CreateAsync(music);
        return musicId;
    }

    public async Task WriteFromJsonAsync(string name, string content)
    {
        var extension = Path.GetExtension(name).ToLowerInvariant();
        if (!Array.Exists(allowedScoreExtensions, ext => ext == extension))
        {
            throw new ArgumentException($"Invalid file extension '{extension}'.");
        }
        var nameNoExtension = Path.GetFileNameWithoutExtension(name);
        var outputDirectory = Path.Combine(_musicPath, nameNoExtension);
        var outputFilePath = Path.Combine($"{_musicPath}/{nameNoExtension}", name);
        
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }                
        await using var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
        var byteData = Encoding.UTF8.GetBytes(content);
        await outputFileStream.WriteAsync(byteData);
    }

    public async Task SaveFileAsync(ChunkDto dto)
    {
        await SaveChunkAsync(dto.Name, dto.Id, dto.TotalChunks, dto.Data);
        if (await IsFileCompleteAsync(dto.Name, dto.TotalChunks))
        {
            await ReconstructFileAsync(dto.Name, dto.TotalChunks, dto.MusicId);
        }
    }

    public async Task UpdateDataAsync(MusicEditDto dto)
    {
        var music = await musicRepository.GetByNameAsync(dto.Name);
        var toUpdate = music.GetValueOrThrow();
        toUpdate.Title = dto.Title ?? toUpdate.Title;
        toUpdate.Author = dto.Author ?? toUpdate.Author;
        await musicRepository.UpdateAsync(toUpdate);
    }

    public async Task DeleteAsync(string name)
    {
        await musicRepository.DeleteAsync(name);
        var nameNoExtension = Path.GetFileNameWithoutExtension(name);
        var path = Path.Combine($"{_musicPath}/{nameNoExtension}");
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    public string GetChunkFilePath(string fileName, int chunkNumber, int totalChunks)
    {
        int numberOfDigits = totalChunks.ToString().Length;

        string paddedChunkNumber = chunkNumber.ToString($"D{numberOfDigits}");
        var fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
        return Path.Combine($"{_musicPath}/tmp/{fileNameNoExtension}", $"{fileName}_chunk_{paddedChunkNumber}.tmp");
    }

    public async Task SaveChunkAsync(string fileName, int chunkNumber, int totalChunks, IFormFile chunkData)
    {
        string chunkFilePath = GetChunkFilePath(fileName, chunkNumber, totalChunks);

        var chunkDirectory = Path.GetDirectoryName(chunkFilePath);
        if (!Directory.Exists(chunkDirectory))
        {
            Directory.CreateDirectory(chunkDirectory);
        }

        await using var fileStream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write);
        await chunkData.CopyToAsync(fileStream);
    }

    public async Task<bool> IsFileCompleteAsync(string fileIdentifier, int totalChunks)
    {
        var chunkFileNames = Enumerable.Range(1, totalChunks)
        .Select(chunkNumber => GetChunkFilePath(fileIdentifier, chunkNumber, totalChunks));
        return await Task.Run(() => chunkFileNames.All(File.Exists));
    }

    public async Task ReconstructFileAsync(string fileName, int totalChunks, int musicId)
    {
        var chunkFileNames = Enumerable.Range(1, totalChunks)
                                        .Select(chunkNumber => GetChunkFilePath(fileName, chunkNumber, totalChunks))
                                        .OrderBy(chunkFilePath => chunkFilePath);

        var nameNoExtension = Path.GetFileNameWithoutExtension(fileName);
        var outputDirectory = Path.Combine(_musicPath, nameNoExtension);
        if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

        var fileFromRepository = await musicRepository.GetByIdAsync(musicId);
        var publishedAt = fileFromRepository.GetValueOrDefault().PublishedAt;
        var fileNameWithPublishedAt = GetFileNameWithPublishedAt(fileName, publishedAt);
        var outputFilePath = Path.Combine(outputDirectory, fileNameWithPublishedAt);
        await using var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
        foreach (var chunkFilePath in chunkFileNames)
        {
            await using var chunkFileStream = new FileStream(chunkFilePath, FileMode.Open, FileAccess.Read);
            await chunkFileStream.CopyToAsync(outputFileStream);
        }
        var chunksDir = Path.Combine($"{_musicPath}/tmp/{nameNoExtension}");
        await DeleteTempChunksDirectoryAsync(chunksDir);
    }

    public async Task DeleteTempChunksDirectoryAsync(string directory)
    {
        if (Directory.Exists(directory)) {
            await Task.Run(() => Directory.Delete(directory, recursive: true));
        }
    }

    public string GetFileNameWithPublishedAt(string fileName, DateTime publishedAt)
    {
        var fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var publishedAtFormatted = publishedAt.ToString("yyyyMMdd_HHmmss");
        var nameWithPublishedAt = $"{fileNameNoExtension}_{publishedAtFormatted}{extension}";
        return nameWithPublishedAt;
    }
}