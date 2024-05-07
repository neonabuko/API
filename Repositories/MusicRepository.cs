using Microsoft.EntityFrameworkCore;
using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Repositories;

public class MusicRepository<T>(DbContext _context) : IMusicRepository<T> where T : Music
{
    protected readonly DbSet<T> _dbSet = _context.Set<T>();

    public async Task<int> CreateAsync(T music)
    {
        await _dbSet.AddAsync(music);
        await _context.SaveChangesAsync();
        return music.Id;
    }

    public async Task<ICollection<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<Optional<T>> GetAsync(int id)
    {
        var music = await _dbSet.FindAsync(id);
#pragma warning disable CS8604 // Possible null reference argument.
        return Optional<T>.FromNullable(music);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    public async Task<Optional<T>> GetByNameAsync(string name)
    {
        var music = await _dbSet.FirstOrDefaultAsync(m => m.Name == name);
#pragma warning disable CS8604 // Possible null reference argument.
        return Optional<T>.FromNullable(music);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    public async Task UpdateAsync(T music)
    {
        _dbSet.Update(music);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var music = await GetAsync(id);
        if (music.HasValue) {
            _dbSet.Remove(music.GetValueOrThrow());
            await _context.SaveChangesAsync();            
        }
    }

    public async Task DeleteByNameAsync(string name)
    {
        var music = await GetByNameAsync(name);
        if (music.HasValue)
        {
            _dbSet.Remove(music.GetValueOrThrow());
            await _context.SaveChangesAsync();
        }
    }
}