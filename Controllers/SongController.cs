using Microsoft.AspNetCore.Mvc;
using Repositories;
using SongManager.Entities;

namespace SongManager;

[ApiController]
public class SongController(SongRepository repository) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Index() => Ok();


    [HttpGet("/songs")]
    public IActionResult GetAllSongs()
    {
        var songsDirectory = new DirectoryInfo("/app/songs");

        if (!songsDirectory.Exists)
        {
            return NotFound();
        }

        var songFiles = songsDirectory.GetFiles();

        var songUrls = new List<string>();
        foreach (var file in songFiles)
        {
            var songUrl = $"http://localhost:5000/songs/{file.Name}"; // Replace with your actual domain
            songUrls.Add(songUrl);
        }

        return Ok(songUrls);
    }




    [HttpPost("/upload")]
    public async Task<IActionResult> Create([FromForm] IFormFile file)
    {
        var songsPath = "/app/songs/" + file.FileName;
        using (var stream = new FileStream(songsPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok();
    }

    [HttpGet("/{id}")]
    public async Task<Song> GetAsync(int id) => await repository.GetAsync(id);
}

