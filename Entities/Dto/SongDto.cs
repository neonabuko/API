namespace SongManager.Entities.Dto;

public record SongDto(
    string Name,
    string? Author,
    TimeSpan Duration
);
