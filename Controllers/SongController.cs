using System.Data;
using Microsoft.AspNetCore.Mvc;
using Service;
using SongManager.Entities.Dto;

namespace SongManager;

[ApiController]
public class SongController(SongService songService, ChunkService chunkService) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Index() => Ok();


    [HttpGet("/songs")]
    public async Task<ICollection<SongDto>> GetAllSongDataAsync() => await songService.GetAllSongData();


    [HttpGet("/songs/{songName}")]
    public IActionResult GetSongFileAsync(string songName)
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
            return StatusCode(409, "Song already exists.");
        }
        catch (NullReferenceException) { }

        await chunkService.StoreChunkAsync(chunkDto.Name, chunkDto.Id, chunkDto.Data);
        if (await chunkService.IsFileCompleteAsync(chunkDto.Name, chunkDto.TotalChunks))
        {
            var bitrate = await chunkService.ReconstructFileAsync(chunkDto.Name, chunkDto.TotalChunks);
            return Ok(new { Bitrate = bitrate });
        }
        return StatusCode(202, "Stored chunk " + chunkDto.Id);
    }


    [HttpPatch("/songs")]
    public async Task<IActionResult> UpdateAsync(SongEditDto songEditDto, [FromQuery] string name)
    {
        try
        {
            await songService.UpdateSongDataAsync(songEditDto, name);
        }
        catch (NullReferenceException e)
        {
            return StatusCode(404, e);
        }
        catch (DBConcurrencyException e)
        {
            return StatusCode(500, e);
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

