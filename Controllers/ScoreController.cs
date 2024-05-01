using Microsoft.AspNetCore.Mvc;
using Repositories;
using Service;
using SongManager.Entities;
using SongManager.Entities.Dto;
using SongManager.Entities.Extensions;

namespace SongManager.Controllers;

[ApiController]
public class ScoreController(ScoreRepository scoreRepository) : ControllerBase
{

    private readonly ChunkService chunkService = new("/app/scores");

    [HttpGet("/scores/data")]
    public async Task<IActionResult> GetAllScoreDataAsync()
    {
        var scores = await scoreRepository.GetAllAsync();
        return Ok(scores);
    }

    [HttpGet("/scores/{name}/data")]
    public async Task<IActionResult> GetScoreDataByNameAsync(string name)
    {
        var score = await scoreRepository.GetByNameAsync(name);
        return Ok(score.AsDto());
    }

    [HttpPost("/scores/data")]
    public async Task<IActionResult> SaveScoreDataAsync([FromForm] ScoreDto scoreDto)
    {
        Score score = new()
        {
            Name = scoreDto.Name,
            Title = scoreDto.Title,
            Author = scoreDto.Author ?? "Unknown"
        };

        await scoreRepository.CreateAsync(score);
        return Ok();
    }

    [HttpPost("/scores/chunks")]
    public async Task<IActionResult> SaveScoreFileAsync([FromForm] ChunkDto chunkDto)
    {
        try
        {
            await scoreRepository.GetByNameAsync(chunkDto.Name);
            return StatusCode(409, "Score already exists.");
        }
        catch (NullReferenceException) { }

        await chunkService.StoreChunkAsync(chunkDto.Name, chunkDto.Id, chunkDto.TotalChunks, chunkDto.Data);
        if (await chunkService.IsFileCompleteAsync(chunkDto.Name, chunkDto.TotalChunks))
        {
            await chunkService.ReconstructFileAsync(chunkDto.Name, chunkDto.TotalChunks);
            return Ok();
        }
        return StatusCode(202, "Stored chunk " + chunkDto.Id);
    }

    [HttpGet("/scores/{name}")]
    public IActionResult GetScoreFileByNameAsync(string name)
    {
        string scorePath = Path.Combine("/app/scores", name);
        var fileStream = new FileStream(scorePath, FileMode.Open, FileAccess.Read);
        return new FileStreamResult(fileStream, "application/vnd.recordare.musicxml+xml");
    }

    [HttpPost("/scores")]
    public async Task<IActionResult> SaveScoreFileAsync([FromForm] IFormFile scoreFile)
    {
        var scorePath = Path.Combine("/app/scores" + $"/{scoreFile.FileName}");
        await using var fileStream = new FileStream(scorePath, FileMode.Create, FileAccess.Write);
        await scoreFile.CopyToAsync(fileStream);
        return StatusCode(201);
    }

    [HttpDelete("/scores/{name}")]
    public async Task<IActionResult> DeleteScoreDataAsync(string name)
    {
        await scoreRepository.DeleteAsync(name);
        var scorePath = Path.Combine($"/app/scores/{name}");
        if (System.IO.File.Exists(scorePath))
        {
            System.IO.File.Delete(scorePath);
            return Ok();
        }
        return NotFound(name);
    }
}