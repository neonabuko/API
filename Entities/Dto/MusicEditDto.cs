namespace ScoreHubAPI.Entities.Dto;

public record MusicEditDto (
    int Id,
    string Name,
    string? Title,
    string? Author
);