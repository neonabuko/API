using Microsoft.AspNetCore.Mvc;
using Repositories;
using SongManager;
using SongManager.Entities;

namespace Service;

public class SongService(IConfiguration configuration, SongRepository songRepository)
{
    private readonly string _storagePath = configuration.GetValue<string>("StoragePath") ?? throw new NullReferenceException();

    public async Task<string> SaveToFileAsync(IFormFile file, string fileName, string author)
    {
        string filePath = Path.Combine(_storagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        await SaveToRepositoryAsync(filePath, fileName, author);
        return fileName;
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

    public IActionResult GetAsync(string songName)
    {
        string filePath = Path.Combine(_storagePath, songName);

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "audio/mpeg");
    }

    public async Task<ICollection<SongDto>> GetAllAsync()
    {
        var songs = await songRepository.GetAllAsync();
        return songs.Select(s => s.AsDto()).ToList();
    }

    public async Task Delete(string songName)
    {
        await songRepository.DeleteAsync(songName);
        string filePath = Path.Combine(_storagePath, songName);
        File.Delete(filePath);
    }
}