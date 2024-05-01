using SongManager.Entities;

namespace Repositories;

public interface IMusicRepository<T> where T : Music
{
    Task CreateAsync(T music);
    Task<ICollection<T>> GetAllAsync();
    Task<T> GetByNameAsync(string name);
    Task UpdateAsync(string name, string title, string author);
    Task DeleteAsync(string name);
}