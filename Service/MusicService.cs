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
        return music;
    }

    public FileStream GetFileByNameAsync(string name)
    {
        string path = Path.Combine(_musicPath, name);
        var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return fileStream;
    }

    public async Task SaveDataAsync(T music)
    {
        await musicRepository.CreateAsync(music);
    }

    public async Task SaveScoreContentAsync(string name, string content)
    {
        var extension = Path.GetExtension(name).ToLowerInvariant();
        if (!Array.Exists(allowedScoreExtensions, ext => ext == extension))
        {
            throw new ArgumentException($"Invalid file extension '{extension}'.");
        }
        var outputFilePath = Path.Combine(_musicPath, $"{name}");
        await using var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
        var byteData = Encoding.UTF8.GetBytes(content);
        await outputFileStream.WriteAsync(byteData);
    }

    public async Task SaveFileAsync(ChunkDto dto)
    {
        await SaveChunkAsync(dto.Name, dto.Id, dto.TotalChunks, dto.Data);
        if (await IsFileCompleteAsync(dto.Name, dto.TotalChunks))
        {
            await ReconstructFileAsync(dto.Name, dto.TotalChunks);
        }
    }

    public async Task UpdateDataAsync(T music)
    {
        var toUpdate = await musicRepository.GetByNameAsync(music.Name);
        toUpdate.Title = music.Title ?? toUpdate.Title;
        toUpdate.Author = music.Author ?? toUpdate.Author;
        await musicRepository.UpdateAsync(toUpdate);
    }

    public async Task DeleteAsync(string name)
    {
        await musicRepository.DeleteAsync(name);
        var path = Path.Combine($"{_musicPath}/{name}");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public async Task SaveChunkAsync(string fileIdentifier, int chunkNumber, int totalChunks, IFormFile chunkData)
    {
        string chunkFilePath = GetChunkFilePath(fileIdentifier, chunkNumber, totalChunks);
        await using var fileStream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write);
        await chunkData.CopyToAsync(fileStream);
    }

    public string GetChunkFilePath(string fileIdentifier, int chunkNumber, int totalChunks)
    {
        int numberOfDigits = totalChunks.ToString().Length;

        string paddedChunkNumber = chunkNumber.ToString($"D{numberOfDigits}");
        return Path.Combine($"{_musicPath}/tmp", $"{fileIdentifier}_chunk_{paddedChunkNumber}.tmp");
    }

    public async Task<bool> IsFileCompleteAsync(string fileIdentifier, int totalChunks)
    {
        var chunkFileNames = Enumerable.Range(1, totalChunks)
        .Select(chunkNumber => GetChunkFilePath(fileIdentifier, chunkNumber, totalChunks));
        return await Task.Run(() => chunkFileNames.All(File.Exists));
    }

    public async Task ReconstructFileAsync(string fileIdentifier, int totalChunks)
    {
        var chunkFileNames = Enumerable.Range(1, totalChunks)
                                        .Select(chunkNumber => GetChunkFilePath(fileIdentifier, chunkNumber, totalChunks))
                                        .OrderBy(chunkFilePath => chunkFilePath);

        var outputFilePath = Path.Combine($"{_musicPath}/{fileIdentifier}");
        await using var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
        foreach (var chunkFilePath in chunkFileNames)
        {
            await using var chunkFileStream = new FileStream(chunkFilePath, FileMode.Open, FileAccess.Read);
            await chunkFileStream.CopyToAsync(outputFileStream);
        }
        await DeleteTempChunksAsync(fileIdentifier, totalChunks);
    }

    public async Task DeleteTempChunksAsync(string fileIdentifier, int totalChunks)
    {
        var chunkFileNames = Enumerable.Range(1, totalChunks)
                                        .Select(chunkNumber => GetChunkFilePath(fileIdentifier, chunkNumber, totalChunks))
                                        .OrderBy(chunkFilePath => chunkFilePath);

        foreach (var chunkFileName in chunkFileNames)
        {
            if (File.Exists(chunkFileName))
            {
                await Task.Run(() => File.Delete(chunkFileName));
            }
        }
    }    
}