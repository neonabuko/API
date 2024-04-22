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
    public async Task<ICollection<SongViewDto>> GetAllSongDataAsync() => await songService.GetAllSongData();


    [HttpGet("/songs/{songName}")]
    public IActionResult GetSongFileAsync(string songName)
    {
        var file = songService.GetSongFileStream(songName);
        return new FileStreamResult(file, "audio/mpeg");
    }


    [HttpPost("/upload")]
    public async Task<IActionResult> CreateAsync([FromForm] SongDto songDto)
    {
        await songService.SaveSongAsync(songDto);
        return Ok();
    }


    [HttpDelete("/delete/{songName}")]
    public async Task<IActionResult> Delete(string songName)
    {
        await songService.DeleteAsync(songName);
        return Ok();
    }
}

