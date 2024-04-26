using Microsoft.EntityFrameworkCore;
using SongManager;
using SongManager.Entities;

namespace Repositories;

public class SongRepository(SongManagerContext context)
{
    public async Task CreateAsync(Song song)
    {
        try
        {
            await GetByNameAsync(song.Name);
            throw new Exception("Song with same name already exists in repository.");
        }
        catch (NullReferenceException)
        {
            await context.AddAsync(song);
            await context.SaveChangesAsync();            
        }
    }

    public async Task<ICollection<Song>> GetAllAsync() => await context.Set<Song>().ToListAsync();

    public async Task<Song> GetAsync(int id)
    {
        return await context.Set<Song>().FindAsync(id) ?? throw new NullReferenceException();
    }

    public async Task<Song> GetByNameAsync(string name)
    {
        return await context.Set<Song>().FirstOrDefaultAsync(s => s.Name == name) ?? 
        throw new NullReferenceException(name + " not found in repository");
    }

    public async Task DeleteAsync(string name)
    {
        var toDelete = await GetByNameAsync(name);
        context.Set<Song>().Remove(toDelete);
        await context.SaveChangesAsync();
    }
}