namespace SongManager.Entities.Dto;

public record ChunkDto(
    int Id,
    string Name,
    IFormFile Data,
    int TotalChunks
);