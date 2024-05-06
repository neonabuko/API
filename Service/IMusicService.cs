using ScoreHubAPI.Entities;
using ScoreHubAPI.Entities.Dto;

namespace ScoreHubAPI.Service;

public interface IMusicService<T> where T : Music
{
    Task<ICollection<T>> GetAllDataAsync();
    Task<T> GetDataByNameAsync(string name);
    FileStream GetFileByNameAsync(string name);
    Task SaveDataAsync(T music);
    Task SaveFileAsync(ChunkDto dto);
    Task UpdateDataAsync(MusicEditDto music);
    Task DeleteAsync(string name);

    // Chunk operations
    Task SaveChunkAsync(string fileIdentifier, int chunkNumber, int totalChunks, IFormFile chunkData);
    Task<bool> IsFileCompleteAsync(string fileIdentifier, int totalChunks);
    Task ReconstructFileAsync(string fileIdentifier, int totalChunks);
    Task DeleteTempChunksAsync(string fileIdentifier, int totalChunks);
    string GetChunkFilePath(string fileIdentifier, int chunkNumber, int totalChunks);
}