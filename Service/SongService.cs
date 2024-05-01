using Repositories;
using SongManager.Entities;
using SongManager.Entities.Dto;
using SongManager.Entities.Extensions;

namespace Service;

public class SongService(IConfiguration configuration, SongRepository songRepository)
{
    private readonly string _songsPath = configuration.GetValue<string>("SongsPath") ?? throw new NullReferenceException();

    public async Task SaveToRepositoryAsync(SongDto songDto)
    {
        Song newSong = new()
        {
            Name = songDto.Name,
            Title = songDto.Title ?? songDto.Name,
            Duration = songDto.Duration,
            Author = songDto.Author ?? "Unknown"
        };
        await songRepository.CreateAsync(newSong);
    }

    public FileStream GetSongFileStream(string name)
    {
        string path = Path.Combine(_songsPath, name);
        var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return fileStream;
    }

    public async Task<SongDto> GetSongDataAsync(string name)
    {
        var song = await songRepository.GetByNameAsync(name);
        return song.AsViewDto();
    }

    public async Task<ICollection<SongDto>> GetAllSongDataAsync()
    {
        var songs = await songRepository.GetAllAsync();
        return songs.Select(s => s.AsViewDto()).ToList();
    }

    public async Task UpdateSongDataAsync(SongEditDto songEditDto, string name)
    {
        var title = songEditDto.Title ?? "";
        var author = songEditDto.Author ?? "";
        await songRepository.UpdateAsync(name, title, author);
    }

    public async Task DeleteAsync(string songName)
    {
        await songRepository.DeleteAsync(songName);
        string filePath = Path.Combine(_songsPath, songName);
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}


