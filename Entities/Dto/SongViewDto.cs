namespace SongManager.Entities.Dto;

public record SongViewDto(
    string Name,
    string? Author,
    TimeSpan Duration,
    int Bitrate
);