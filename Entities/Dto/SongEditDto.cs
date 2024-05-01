namespace SongManager.Entities.Dto;

public record SongEditDto (
    string Name,
    string? Title,
    string? Author
);