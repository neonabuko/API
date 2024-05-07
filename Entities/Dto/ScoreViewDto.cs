namespace ScoreHubAPI.Entities.Dto;

public record ScoreViewDto (
    int Id,
    string Name,
    string Title,
    string Author,
    DateTime PublishedAt
);