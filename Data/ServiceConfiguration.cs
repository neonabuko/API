using Microsoft.EntityFrameworkCore;
using ScoreHubAPI.Repositories;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities;
using ScoreHubAPI.Rules;

namespace ScoreHubAPI.Data;

public static class ServiceConfiguration
{

    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScoreHubContext>();
        await dbContext.Database.MigrateAsync();
    } 

    public static IServiceCollection ConfigureServices(this IServiceCollection services,
    IConfiguration configuration) 
    {
        var connectionString = configuration.GetConnectionString("ScoreHubContext");
        services.AddSingleton(configuration);
        services.AddSqlServer<ScoreHubContext>(connectionString);
        
        services.AddScoped<SongRepository>();
        services.AddScoped<ScoreRepository>();
        services.AddScoped<IMusicRepository<Song>, SongRepository>();
        services.AddScoped<IMusicRepository<Score>, ScoreRepository>();

        var scoresPath = configuration.GetValue<string>("StoragePaths:scores");
        var songsPath = configuration.GetValue<string>("StoragePaths:songs");

        services.AddScoped<ScoreService>(sp => new ScoreService(
            sp.GetRequiredService<IMusicRepository<Score>>(), scoresPath ?? throw new NullReferenceException()
        ));
        
        services.AddScoped<SongService>(sp => new SongService(
            sp.GetRequiredService<IMusicRepository<Song>>(), songsPath ?? throw new NullReferenceException()
        ));

        // services.AddScoped<SongRules>(sr => new SongRules(
        //     sr.GetRequiredService<IMusicRepository<Song>>()
        // ));
        // services.AddScoped<ScoreRules>(sr => new ScoreRules(
        //     sr.GetRequiredService<IMusicRepository<Score>>()
        // ));

        services.AddScoped<SongRules>();
        services.AddScoped<ScoreRules>();

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
