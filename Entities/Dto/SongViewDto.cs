namespace SongManager;

public record SongViewDto(
    string Name,
    string? Author,
    TimeSpan Duration
);