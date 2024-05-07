using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Data;

public class ScoreHubContext(DbContextOptions<ScoreHubContext> options) : DbContext(options)
{
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<Score> Scores => Set<Score>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
