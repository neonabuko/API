namespace ScoreHubAPI.Entities.Dto;

public record SongDto(
    int Id,
    string Name,
    string Title,
    string? Author,
    TimeSpan Duration,
    DateTime PublishedAt
);
