using ScoreHubAPI.Controllers.ErrorHandling;
using ScoreHubAPI.Entities;
using ScoreHubAPI.Repositories;

namespace ScoreHubAPI.Rules;

public interface IMusicRules<T> where T : Music
{
    Task HandleSave(T music);
}

public class MusicRules<T>(IMusicRepository<T> musicRepository) : IMusicRules<T> where T : Music
{
    public async Task HandleSave(T music)
    {
        var name = music.Name;
        if (string.IsNullOrWhiteSpace(name) || name == "undefined")
        {
            throw new ArgumentNullException(nameof(music), "Must provide file.");
        }
        try
        {
            var existent = await musicRepository.GetByNameAsync(name);
            throw new ConflictException("Music already exists.");
        }
        catch (NullReferenceException){}
    }
}

public class SongRules(IMusicRepository<Song> songRepository) : MusicRules<Song>(songRepository)
{ }

public class ScoreRules(IMusicRepository<Score> scoreRepository) : MusicRules<Score>(scoreRepository)
{ }