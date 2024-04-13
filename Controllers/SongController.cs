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
        string filePath = Path.GetTempFileName();
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var blobUrl = await blobStorageService.UploadSongAsync(filePath, file.FileName);

        System.IO.File.Delete(filePath);

        return Ok(blobUrl);
    }
}

