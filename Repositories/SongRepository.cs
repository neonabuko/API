using Microsoft.EntityFrameworkCore;
using SongManager;
using SongManager.Entities;
using SongManager.Entities.Dto;

namespace Repositories;

public class SongRepository(SongManagerContext context)
{
    public async Task CreateAsync(Song song)
    {
        try
        {
            await GetByNameAsync(song.Name);
            throw new InvalidOperationException("Song with same name already exists in repository.");
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

    public async Task UpdateAsync(SongEditDto songEditDto, string name)
    {
        var songInRepo = await GetByNameAsync(name);

        songInRepo.Title = songEditDto.Title ?? throw new ArgumentException("Must provide song title");
        songInRepo.Author = songEditDto.Author ?? songInRepo.Author;

        try
        {
            context.Set<Song>().Update(songInRepo);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Failed to update the song due to concurrency issues.", ex);
        }
    }


    public async Task DeleteAsync(string name)
    {
        var toDelete = await GetByNameAsync(name);
        context.Set<Song>().Remove(toDelete);
        await context.SaveChangesAsync();
    }
}