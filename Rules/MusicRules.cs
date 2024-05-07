using ScoreHubAPI.Entities;
using ScoreHubAPI.Entities.Dto;

namespace ScoreHubAPI.Rules;

public interface IMusicRules<T> where T : Music
{
    void HandleSaveAsync(T music);
    void HandleUpdateData(MusicEditDto dto);
}

public class MusicRules<T> : IMusicRules<T> where T : Music
{
    public void HandleSaveAsync(T music)
    {
        if (string.IsNullOrWhiteSpace(music.Name))
        {
            throw new ArgumentNullException(nameof(music), "Must provide file name.");
        }
    }

    public void HandleUpdateData(MusicEditDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentNullException(nameof(dto), "Must provide file name.");
        }
    }
}

public class SongRules : MusicRules<Song>
{ }

public class ScoreRules : MusicRules<Score>
{ }