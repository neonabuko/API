using SongManager.Entities;

namespace Repositories;

public interface IMusicRepository<T> where T : Music
{
    Task CreateAsync(T music);
    Task<ICollection<T>> GetAllAsync();
    Task<T> GetByNameAsync(string name);
    Task UpdateAsync(T music);
    Task DeleteAsync(string name);
}