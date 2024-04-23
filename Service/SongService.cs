using Repositories;
using SongManager.Entities;
using SongManager.Entities.Dto;
using SongManager.Entities.Extensions;

namespace Service;

public class SongService(IConfiguration configuration, SongRepository songRepository)
{
    private readonly string _storagePath = configuration.GetValue<string>("StoragePath") ?? throw new NullReferenceException();

    public async Task SaveToRepositoryAsync(SongDto songDto)
    {
        var filePath = _storagePath + $"/{songDto.Name}";
        using var fileTag = TagLib.File.Create(filePath);
        TimeSpan duration = fileTag.Properties.Duration;
        Song newSong = new()
        {
            Name = songDto.Name,
            Duration = duration,
            Author = songDto.Author
        };
        await songRepository.CreateAsync(newSong);
    }

    public FileStream GetSongFileStream(string name)
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
        try
        {
            await songRepository.DeleteAsync(songName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        string filePath = Path.Combine(_storagePath, songName);
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}