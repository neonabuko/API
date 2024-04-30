using Microsoft.AspNetCore.Mvc;
using Repositories;
using SongManager.Entities;
using SongManager.Entities.Dto;
using SongManager.Entities.Extensions;

namespace SongManager.Controllers;

[ApiController]
public class ScoreController(ScoreRepository scoreRepository) : ControllerBase
{
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
            Author = scoreDto.Author
        };

        await scoreRepository.CreateAsync(score);
        return Ok();
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

    [HttpDelete("/scores/{name}/data")]
    public async Task<IActionResult> DeleteScoreDataAsync(string name)
    {
        await scoreRepository.DeleteAsync(name);
        return Ok();
    }

    [HttpDelete("/scores/{name}")]
    public IActionResult DeleteScoreFileAsync(string name)
    {
        var scorePath = Path.Combine($"/app/scores/{name}");
        if (System.IO.File.Exists(scorePath)) {
            System.IO.File.Delete(scorePath);
            return Ok();
        }
        return NotFound(name);
    }
}