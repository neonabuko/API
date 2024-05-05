using ScoreHubAPI.Entities.Dto;

namespace ScoreHubAPI.Entities.Extensions;

public static class EntityExtensions
{
    public static SongDto AsDto(this Song song) 
    {
        return new SongDto(
            song.Name,
            song.Title,
            song.Author,
            song.Duration
        );
    }

    public static ScoreViewDto AsViewDto(this Score score)
    {
        return new ScoreViewDto(
            score.Name,
            score.Title,
            score.Author
        );
    }
}
