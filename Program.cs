using Microsoft.AspNetCore.StaticFiles;
using SongManager.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings = {
            [".mp3"] = "audio/mpeg"
        }
    }
});

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}"
    );

app.UseCors("AllowAllOrigins");

await app.Services.InitializeDbAsync();

app.Run();
