using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repositories;
using Service;

namespace SongManager.Data;

public static class ServiceConfiguration
{

    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
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
