using Microsoft.EntityFrameworkCore;
using Repositories;
using Service;
using SongManager.Entities;

namespace SongManager.Data;

public static class ServiceConfiguration
{

    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SongManagerContext>();
        await dbContext.Database.MigrateAsync();
    } 

    public static IServiceCollection ConfigureServices(this IServiceCollection services,
    IConfiguration configuration) 
    {
        var connectionString = configuration.GetConnectionString("SongManagerContext");
        services.AddSingleton(configuration);
        services.AddSqlServer<SongManagerContext>(connectionString);
        
        services.AddScoped<SongRepository>();
        services.AddScoped<ScoreRepository>();
        services.AddScoped<IMusicRepository<Song>, SongRepository>();
        services.AddScoped<IMusicRepository<Score>, ScoreRepository>();
        services.AddScoped<SongService>();
        services.AddScoped<ChunkService>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

}
