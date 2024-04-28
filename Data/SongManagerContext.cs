using System.Reflection;
using Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using SongManager.Entities;

namespace SongManager.Data;

public class SongManagerContext(DbContextOptions<SongManagerContext> options) : DbContext(options)
{
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<Score> Scores => Set<Score>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyConfiguration(new SongConfiguration());
    }
}
