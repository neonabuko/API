using Microsoft.EntityFrameworkCore;
using SongManager.Data;
using SongManager.Entities;

namespace Repositories;

public class ScoreRepository(SongManagerContext context) {
    public async Task CreateAsync(Score score)
    {
        await context.AddAsync(score);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<Score>> GetAllAsync() => await context.Set<Score>().ToListAsync();

    public async Task<Score> GetByNameAsync(string name)
    {
        return await context.Set<Score>().FirstOrDefaultAsync(s => s.Name == name) ??
        throw new NullReferenceException($"Score '{name}' not found in repository.");
    }

    public async Task DeleteAsync(string name)
    {
        var toDelete = await GetByNameAsync(name);
        context.Set<Score>().Remove(toDelete);
        await context.SaveChangesAsync();
    }
}