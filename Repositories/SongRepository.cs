using Microsoft.EntityFrameworkCore;
using SongManager;
using SongManager.Entities;

namespace Repositories;

public class SongRepository(SongManagerContext context) 
{
    public async Task CreateAsync(Song song)
    {
        await context.AddAsync(song);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<Song>> GetAllAsync() => await context.Set<Song>().ToListAsync();

    public async Task<Song> GetAsync(int id)
    {
        return await context.Set<Song>().FindAsync(id) ?? throw new NullReferenceException();
    }

    public async Task<Song> GetByNameAsync(string name)
    {
        return await context.Set<Song>().Where(s => s.Name == name).FirstAsync();
    }

    public async Task DeleteAsync(string name) => await context.Set<Song>().Where(s => s.Name == name).ExecuteDeleteAsync();
}