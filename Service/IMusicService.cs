using ScoreHubAPI.Entities;
using ScoreHubAPI.Entities.Dto;

namespace ScoreHubAPI.Service;

public interface IMusicService<T> where T : Music
{
    Task<ICollection<T>> GetAllDataAsync();
    Task<T> GetDataByIdAsync(int id);
    Task<int> SaveDataAsync(T music);
    Task SaveFileAsync(ChunkDto dto);
    Task UpdateDataAsync(MusicEditDto music);
    Task DeleteAsync(int id);

    // Chunk operations
    Task SaveChunkAsync(ChunkDto dto);
    Task<bool> IsFileCompleteAsync(string fileIdentifier, int totalChunks);
    Task ReconstructFileAsync(string fileIdentifier, int totalChunks, int musicId);
    Task DeleteTempChunksDirectoryAsync(string directory);
    string GetChunkFilePath(string fileIdentifier, int chunkNumber, int totalChunks);
}