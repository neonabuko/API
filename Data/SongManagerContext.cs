using System.Reflection;
using Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using SongManager.Entities;

namespace SongManager;

public class SongManagerContext(DbContextOptions<SongManagerContext> options) : DbContext(options)
{
    public DbSet<Song> Songs => Set<Song>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyConfiguration(new SongConfiguration());
    }
}
