using Microsoft.AspNetCore.Mvc;
using Service;
using SongManager.Entities.Dto;

namespace SongManager.Controllers;

[ApiController]
public class SongController(SongService songService) : ControllerBase
{
    private readonly ChunkService chunkService = new("/app/songs");

    [HttpGet("/")]
    public IActionResult Index() => Ok();


    [HttpGet("/songs")]
    public async Task<ICollection<SongDto>> GetAllSongDataAsync() => await songService.GetAllSongDataAsync();

    [HttpGet("/songs/{songName}/data")]
    public async Task<IActionResult> GetSongAsync(string songName)
    {
        var song = await songService.GetSongDataAsync(songName);
        return Ok(song);
    }


    [HttpGet("/songs/{songName}")]
    public IActionResult StreamSongAsync(string songName)
    {
        var fileStream = songService.GetSongFileStream(songName);
        if (fileStream == null)
        {
            return NotFound();
        }

        long fileSize = fileStream.Length;

        if (Request.Headers.TryGetValue("Range", out var rangeValues))
        {
            var rangeHeader = rangeValues.FirstOrDefault();

            if (rangeHeader != null && rangeHeader.StartsWith("bytes="))
            {
                var rangeParts = rangeHeader[6..].Split('-');
                long startByte = long.Parse(rangeParts[0]);
                long endByte = (rangeParts.Length > 1 && !string.IsNullOrWhiteSpace(rangeParts[1]))
                                ? long.Parse(rangeParts[1])
                                : fileSize - 1;

                var responseLength = endByte - startByte + 1;

                fileStream.Seek(startByte, SeekOrigin.Begin);

                var responseStream = new FileStreamResult(fileStream, "audio/mpeg")
                {
                    EnableRangeProcessing = true
                };

                Response.StatusCode = 206;
                Response.Headers.ContentRange = $"bytes {startByte}-{endByte}/{fileSize}";
                Response.Headers["Content-Length"] = responseLength.ToString();

                return responseStream;
            }
        }

        return new FileStreamResult(fileStream, "audio/mpeg")
        {
            EnableRangeProcessing = true
        };
    }

    [HttpPost("/songs/data")]
    public async Task<IActionResult> SaveSongDataAsync([FromForm] SongDto songDto)
    {
        await songService.SaveToRepositoryAsync(songDto);
        return Ok();
    }

    [HttpPost("/songs/chunks")]
    public async Task<IActionResult> SaveSongFileAsync([FromForm] ChunkDto chunkDto)
    {
        try
        {
            await songService.GetSongDataAsync(chunkDto.Name);
            return StatusCode(409, "Song already exists.");
        }
        catch (NullReferenceException) { } // Song doesn't exist yet in repository so we can save it

        await chunkService.StoreChunkAsync(chunkDto.Name, chunkDto.Id, chunkDto.TotalChunks, chunkDto.Data);
        if (await chunkService.IsFileCompleteAsync(chunkDto.Name, chunkDto.TotalChunks))
        {
            await chunkService.ReconstructFileAsync(chunkDto.Name, chunkDto.TotalChunks);
            return Ok();
        }
        return StatusCode(202, "Stored chunk " + chunkDto.Id);
    }


    [HttpPatch("/songs/data")]
    public async Task<IActionResult> UpdateSongDataAsync([FromForm] SongEditDto songEditDto)
    {
        try
        {
            await songService.UpdateSongDataAsync(songEditDto);
        }
        catch (NullReferenceException e)
        {
            return StatusCode(404, e);
        }
        catch (InvalidOperationException e)
        {
            return StatusCode(500, e);
        }
        return Ok();
    }

    [HttpDelete("/songs/{songName}")]
    public async Task<IActionResult> DeleteSongAsync(string songName)
    {
        await songService.DeleteAsync(songName);
        return Ok();
    }
}

