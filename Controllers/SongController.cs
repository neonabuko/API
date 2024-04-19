using Microsoft.AspNetCore.Mvc;
using Repositories;
using Service;

namespace SongManager;

[ApiController]
public class SongController(SongService songService) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Index() => Ok();


    [HttpGet("/songs")]
    public async Task<IEnumerable<SongDto>> GetAllSongsAsync() => await songService.GetAllAsync();


    [HttpGet("/songs/{songName}")]
    public IActionResult GetAsync(string songName)
    {
        var file = songService.GetAsync(songName);
        return new FileStreamResult(file, "audio/mpeg");
    }


    [HttpPost("/upload")]
    public async Task<IActionResult> CreateAsync([FromForm] IFormFile file, [FromForm] string author)
    {
        await songService.SaveToFileAsync(file, file.FileName, author);
        return Ok();
    }


    [HttpDelete("/delete/{songName}")]
    public async Task<IActionResult> Delete(string songName)
    {
        await songService.DeleteAsync(songName);
        return Ok();
    }
}

