using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Repositories;

public interface IMusicRepository<T> where T : Music
{
    Task<int> CreateAsync(T music);
    Task<ICollection<T>> GetAllAsync();
    Task<Optional<T>> GetByIdAsync(int id); 
    Task<Optional<T>> GetByNameAsync(string name);
    Task UpdateAsync(T music);
    Task DeleteAsync(string name);
}