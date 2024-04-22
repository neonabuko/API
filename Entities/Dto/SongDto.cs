namespace SongManager;

public record SongDto(
    IFormFile File,
    string? Author
);
