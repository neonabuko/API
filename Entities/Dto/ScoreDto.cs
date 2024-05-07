namespace ScoreHubAPI.Entities.Dto;

public record ScoreDto(
    string Name,
    string Title,
    string? Author,
    string? Content,
    DateTime PublishedAt
);