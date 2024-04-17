using Microsoft.AspNetCore.Mvc;
using SongManager.Data;

namespace SongManager;

[ApiController]
public class SongController(BlobStorageService blobStorageService) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Index() => Ok();


    [HttpGet("/songs")]
    public async Task<ICollection<SongDto>> GetAllSongs() => await blobStorageService.GetAllSongsAsync();


    [HttpPost("/upload")]
    public async Task<IActionResult> Create([FromForm] IFormFile file)
    {
        var blobUrl = await blobStorageService.UploadSongAsync(file, file.FileName);
        return Ok(blobUrl);
    }

    [HttpDelete("/delete/{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        await blobStorageService.DeleteSongAsync(name);
        return Ok();
    }
}

