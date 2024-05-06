using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Repositories;

public interface IMusicRepository<T> where T : Music
{
    Task CreateAsync(T music);
    Task<ICollection<T>> GetAllAsync();
    Task<Optional<T>> GetByNameAsync(string name);
    Task UpdateAsync(T music);
    Task DeleteAsync(string name);
}