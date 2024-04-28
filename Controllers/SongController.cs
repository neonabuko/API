using Microsoft.AspNetCore.Mvc;
using Service;
using SongManager.Entities.Dto;

namespace SongManager.Controllers;

[ApiController]
public class SongController(SongService songService, ChunkService chunkService) : ControllerBase
{
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

    [HttpPost("/songs")]
    public async Task<IActionResult> SaveSongDataAsync([FromForm] SongDto songDto)
    {
        await songService.SaveToRepositoryAsync(songDto);
        return Ok();
    }

    [HttpPost("/songs/uploadChunk")]
    public async Task<IActionResult> SaveSongFileAsync([FromForm] ChunkDto chunkDto)
    {
        try
        {
            await songService.GetSongDataAsync(chunkDto.Name);
            return StatusCode(409, "Song already exists.");
        }
        catch (NullReferenceException) { } // Means song doesn't exist yet in repository so we can save it

        await chunkService.StoreChunkAsync(chunkDto.Name, chunkDto.Id, chunkDto.TotalChunks, chunkDto.Data);
        if (await chunkService.IsFileCompleteAsync(chunkDto.Name, chunkDto.TotalChunks))
        {
            var bitrate = await chunkService.ReconstructFileAsync(chunkDto.Name, chunkDto.TotalChunks);
            return Ok(new { Bitrate = bitrate });
        }
        return StatusCode(202, "Stored chunk " + chunkDto.Id);
    }


    [HttpPatch("/songs")]
    public async Task<IActionResult> UpdateSongDataAsync([FromForm] SongEditDto songEditDto, [FromQuery] string name)
    {
        try
        {
            await songService.UpdateSongDataAsync(songEditDto, name);
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

