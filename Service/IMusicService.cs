using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Service;

public interface IMusicService
{
    Task<ICollection<Music>> GetAllDataAsync();
    Task<Music> GetDataByNameAsync();
}