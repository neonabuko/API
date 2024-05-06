namespace ScoreHubAPI.Entities.Dto;

public record MusicEditDto (
    string Name,
    string? Title,
    string? Author
);