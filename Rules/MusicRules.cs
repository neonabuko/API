using ScoreHubAPI.Controllers.ErrorHandling;
using ScoreHubAPI.Entities;
using ScoreHubAPI.Entities.Dto;
using ScoreHubAPI.Repositories;

namespace ScoreHubAPI.Rules;

public interface IMusicRules<T> where T : Music
{
    Task HandleSaveAsync(T music);
    void HandleUpdateData(MusicEditDto dto);
}

public class MusicRules<T>(IMusicRepository<T> musicRepository) : IMusicRules<T> where T : Music
{
    public async Task HandleSaveAsync(T music)
    {
        if (string.IsNullOrWhiteSpace(music.Name) || music.Name == "undefined")
        {
            throw new ArgumentNullException(nameof(music), "Must provide file name.");
        }
        try
        {
            var existent = await musicRepository.GetByNameAsync(music.Name);
            throw new ConflictException("Music already exists.");
        }
        catch (NullReferenceException) { }
    }

    public void HandleUpdateData(MusicEditDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name == "undefined")
        {
            throw new ArgumentNullException(nameof(dto), "Must provide file name.");
        }
    }
}

public class SongRules(IMusicRepository<Song> songRepository) : MusicRules<Song>(songRepository)
{ }

public class ScoreRules(IMusicRepository<Score> scoreRepository) : MusicRules<Score>(scoreRepository)
{ }