using Microsoft.AspNetCore.Mvc;
using Service;

namespace SongManager;

[ApiController]
public class SongController(SongService songService) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Index() => Ok();


    [HttpGet("/songs")]
    public async Task<IEnumerable<SongDto>> GetAllSongsAsync() => await songService.GetAllSongsAsync();


    [HttpGet("/songs/{songName}")]
    public IActionResult GetAsync(string songName)
    {
        return songService.GetSongAsync(songName);
    }


    [HttpPost("/upload")]
    public async Task<IActionResult> CreateAsync([FromForm] IFormFile file, [FromForm] string fileName)
    {
        await songService.SaveFileAsync(file, fileName);
        return Ok();
    }


    [HttpDelete("/delete/{songName}")]
    public IActionResult Delete(string songName)
    {
        songService.DeleteSong(songName);
        return Ok();
    }
}

