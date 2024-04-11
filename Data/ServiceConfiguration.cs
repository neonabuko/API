using Microsoft.EntityFrameworkCore;
using Repositories;

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
        services.AddSqlServer<SongManagerContext>(connectionString);

        services.AddScoped<SongRepository>();

        services.AddControllers();

        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        return services;
    }

}
