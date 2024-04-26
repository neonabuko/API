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
        Song newSong = new()
        {
            Name = songDto.Name,
            Title = songDto.Title ?? songDto.Name,
            Duration = songDto.Duration,
            Author = songDto.Author,
            Bitrate = songDto.Bitrate
        };
        await songRepository.CreateAsync(newSong);
    }

    public FileStream GetSongFileStream(string name)
    {
        string path = Path.Combine(_storagePath, name);
        var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return fileStream;
    }

    public async Task<SongDto> GetSongDataAsync(string name)
    {
        var song = await songRepository.GetByNameAsync(name);
        return song.AsViewDto();
    }

    public async Task<ICollection<SongDto>> GetAllSongData()
    {
        var songs = await songRepository.GetAllAsync();
        return songs.Select(s => s.AsViewDto()).ToList();
    }

    public async Task UpdateSongDataAsync(SongEditDto songEditDto, string name) {
        await songRepository.UpdateAsync(songEditDto, name);
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


