using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Repositories;

public interface IMusicRepository<T> where T : Music
{
    Task<int> CreateAsync(T music);
    Task<ICollection<T>> GetAllAsync();
    Task<Optional<T>> GetAsync(int id); 
    Task UpdateAsync(T music);
    Task DeleteAsync(int id);
}