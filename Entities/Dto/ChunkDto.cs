namespace ScoreHubAPI.Entities.Dto;

public record ChunkDto (
    int Id,
    int MusicId,
    string Name,
    IFormFile Data,
    int TotalChunks
);