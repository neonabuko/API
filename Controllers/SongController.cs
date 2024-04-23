using Microsoft.AspNetCore.Mvc;
using Repositories;
using Service;
using SongManager.Entities.Dto;

namespace SongManager;

[ApiController]
public class SongController(SongService songService, ChunkService chunkService) : ControllerBase
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
        await songService.SaveToRepositoryAsync(songDto);
        return Ok();
    }

    [HttpPost("/uploadChunk")]
    public async Task<IActionResult> CreateChunkAsync([FromForm] ChunkDto chunkDto)
    {
        try
        {
            await songService.GetSongDataAsync(chunkDto.Name);
            throw new Exception("Song already exists.");
        }
        catch (Exception){}
        await chunkService.StoreChunkAsync(chunkDto.Name, chunkDto.Id, chunkDto.Data);
        if (await chunkService.IsFileCompleteAsync(chunkDto.Name, chunkDto.TotalChunks)) {
            await chunkService.ReconstructFileAsync(chunkDto.Name, chunkDto.TotalChunks);
        }
        return Ok();
    }


    [HttpDelete("/delete/{songName}")]
    public async Task<IActionResult> Delete(string songName)
    {
        await songService.DeleteAsync(songName);
        return Ok();
    }
}

