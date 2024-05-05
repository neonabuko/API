using Microsoft.AspNetCore.Mvc;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities.Dto;
using ScoreHubAPI.Entities.Extensions;
using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Controllers;

[ApiController]
[Route("/songs")]
public class SongController(SongService songService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllSongDataAsync()
    {
        var songs = await songService.GetAllDataAsync();
        var songsDto = songs.Select(s => s.AsDto()).ToList();
        return Ok(songsDto);
    }

    [HttpGet("{songName}/data")]
    public async Task<IActionResult> GetSongAsync(string songName)
    {
        var song = await songService.GetDataByNameAsync(songName);
        return Ok(song);
    }


    [HttpGet("{songName}")]
    public IActionResult StreamSongAsync(string songName)
    {
        var fileStream = songService.GetFileByNameAsync(songName);
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

    [HttpPost("data")]
    public async Task<IActionResult> SaveSongDataAsync([FromForm] SongDto songDto)
    {
        Song song = new()
        {
            Name = songDto.Name,
            Title = songDto.Title,
            Author = songDto.Author ?? "Unknown",
            Duration = songDto.Duration
        };
        await songService.SaveDataAsync(song);
        return Ok();
    }

    [HttpPost("chunks")]
    public async Task<IActionResult> SaveSongFileAsync([FromForm] ChunkDto chunkDto)
    {
        await songService.SaveFileAsync(chunkDto);
        return Ok();
    }


    [HttpPatch("data")]
    public async Task<IActionResult> UpdateDataAsync([FromForm] SongEditDto dto)
    {
        try
        {
            Song song = new()
            {
                Name = dto.Name,
                Title = dto.Title,
                Author = dto.Author
            };
            await songService.UpdateDataAsync(song);
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
        return Ok();
    }

    [HttpDelete("{songName}")]
    public async Task<IActionResult> DeleteSongAsync(string songName)
    {
        await songService.DeleteAsync(songName);
        return Ok();
    }
}

