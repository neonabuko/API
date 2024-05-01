using SongManager.Entities.Dto;

namespace SongManager.Entities.Extensions;

public static class EntityExtensions
{
    public static SongDto AsViewDto(this Song song) 
    {
        return new SongDto(
            song.Name,
            song.Title,
            song.Author,
            song.Duration
        );
    }

    public static ScoreDto AsDto(this Score score)
    {
        return new ScoreDto(
            score.Name,
            score.Title,
            score.Author
        );
    }
}
