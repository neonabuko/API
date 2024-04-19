using Microsoft.AspNetCore.Mvc;
using SongManager;

namespace Service;

public class SongService(IConfiguration configuration)
{
    private readonly string _storagePath = configuration.GetValue<string>("StoragePath") ?? throw new NullReferenceException();

    public async Task<string> SaveFileAsync(IFormFile file, string fileName)
    {
        string filePath = Path.Combine(_storagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    public IActionResult GetSongAsync(string songName)
    {
        string filePath = Path.Combine(_storagePath, songName);

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "audio/mpeg");
    }

    public async Task<IEnumerable<SongDto>> GetAllSongsAsync()
    {
        string[] fileNames = Directory.GetFiles(_storagePath);

        List<SongDto> songs = new List<SongDto>();

        foreach (string fileName in fileNames)
        {
            string path = Path.Combine(_storagePath, fileName);

            using var file = TagLib.File.Create(path);
            TimeSpan duration = file.Properties.Duration;

            songs.Add(new SongDto
            (
                Name: Path.GetFileName(fileName),
                Duration: duration,
                Url: $"/songs/{Path.GetFileName(fileName)}"
            ));
        }

        return await Task.FromResult(songs);
    }

    public void DeleteSong(string songName)
    {
        string filePath = Path.Combine(_storagePath, songName);
        File.Delete(filePath);
    }
}