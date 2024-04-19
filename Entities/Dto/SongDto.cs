namespace SongManager;

public record SongDto(
    string Name,
    TimeSpan Duration,
    string Url
);
