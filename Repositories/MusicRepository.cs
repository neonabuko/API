using Microsoft.EntityFrameworkCore;
using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Repositories;

public class MusicRepository<T>(DbContext _context) : IMusicRepository<T> where T : Music
{
    protected readonly DbSet<T> _dbSet = _context.Set<T>();

    public async Task CreateAsync(T music)
    {
        await _dbSet.AddAsync(music);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<T>> GetAllAsync() => await _dbSet.ToListAsync();
    
    public async Task<T> GetByNameAsync(string name) => await _dbSet.FirstOrDefaultAsync(m => m.Name == name)
    ?? throw new NullReferenceException($"'{name}' not found in repository.");

    public async Task UpdateAsync(T music)
    {
        _dbSet.Update(music);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string name)
    {
        var music = await GetByNameAsync(name);
        if (music != null) {
            _dbSet.Remove(music);
            await _context.SaveChangesAsync();
        }
    }    
}