using Microsoft.AspNetCore.Mvc;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities.Dto;

namespace ScoreHubAPI.Controllers;

[ApiController]
[Route("/songs")]
public class SongController(SongService songService) : ControllerBase
{
    private readonly ChunkService chunkService = new("/app/songs");

    [HttpGet]
    public async Task<ICollection<SongDto>> GetAllSongDataAsync() => await songService.GetAllSongDataAsync();

    [HttpGet("{songName}/data")]
    public async Task<IActionResult> GetSongAsync(string songName)
    {
        var song = await songService.GetSongDataAsync(songName);
        return Ok(song);
    }


    [HttpGet("{songName}")]
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

    [HttpPost("data")]
    public async Task<IActionResult> SaveSongDataAsync([FromForm] SongDto songDto)
    {
        await songService.SaveToRepositoryAsync(songDto);
        return Ok();
    }

    [HttpPost("chunks")]
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


    [HttpPatch("data")]
    public async Task<IActionResult> UpdateDataAsync([FromForm] SongEditDto songEditDto)
    {
        try
        {
            await songService.UpdateSongDataAsync(songEditDto);
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

