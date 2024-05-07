using ScoreHubAPI.Entities.Dto;

namespace ScoreHubAPI.Entities.Extensions;

public static class EntityExtensions
{
    public static SongDto AsDto(this Song song) 
    {
        return new SongDto(
            song.Id,
            song.Name,
            song.Title,
            song.Author,
            song.Duration,
            song.PublishedAt
        );
    }

    public static Song AsSong(this SongDto dto)
    {
        return new() 
        {
            Name = dto.Name,
            Title = dto.Title,
            Author = dto.Author ?? "Unknown",
            Duration = dto.Duration,
            PublishedAt = dto.PublishedAt
        };
    }

    public static ScoreViewDto AsViewDto(this Score score)
    {
        return new ScoreViewDto(
            score.Id,
            score.Name,
            score.Title,
            score.Author,
            score.PublishedAt
        );
    }

    public static Score AsScore(this ScoreDto dto)
    {
        return new()
        {
            Name = dto.Name,
            Title = dto.Title,
            Author = dto.Author ?? "Unknown",
            PublishedAt = dto.PublishedAt
        };
    }
}
