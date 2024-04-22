using Microsoft.AspNetCore.Mvc;
using Repositories;
using SongManager;
using SongManager.Entities;

namespace Service;

public class SongService(IConfiguration configuration, SongRepository songRepository)
{
    private readonly string _storagePath = configuration.GetValue<string>("StoragePath") ?? throw new NullReferenceException();

    public async Task SaveSongAsync(SongDto songDto)
    {
        string filePath = Path.Combine(_storagePath, songDto.File.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await songDto.File.CopyToAsync(stream);
        }
        await SaveToRepositoryAsync(filePath, songDto.File.FileName, songDto.Author ?? "");
    }

    private async Task SaveToRepositoryAsync(string filePath, string fileName, string author)
    {
        using var fileTag = TagLib.File.Create(filePath);
        TimeSpan duration = fileTag.Properties.Duration;
        Song newSong = new()
        {
            Name = fileName,
            Duration = duration,
            Url = $"/songs/{fileName}",
            Author = author
        };
        await songRepository.CreateAsync(newSong);
    }

    public FileStream GetSongFileAsync(string name)
    {
        string path = Path.Combine(_storagePath, name);
        var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return fileStream;
    }

    public async Task<SongViewDto> GetSongDataAsync(string name)
    {
        var song = await songRepository.GetByNameAsync(name);
        return song.AsViewDto();
    }

    public async Task<ICollection<SongViewDto>> GetAllSongData()
    {
        var songs = await songRepository.GetAllAsync();
        return songs.Select(s => s.AsViewDto()).ToList();
    }

    public async Task DeleteAsync(string songName)
    {
        await songRepository.DeleteAsync(songName);
        string filePath = Path.Combine(_storagePath, songName);
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}