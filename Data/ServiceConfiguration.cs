using Microsoft.AspNetCore.Server.Kestrel.Core;
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
        services.AddSingleton(configuration);
        services.AddSqlServer<SongManagerContext>(connectionString);

        services.AddScoped<BlobStorageService>();
        services.AddScoped<BlobContainerConfiguration>();
        services.AddScoped<SongRepository>();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // services.Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));

        // services.AddHttpsRedirection(options => 
        // {
        //     options.HttpsPort = 7196;
        // });

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
