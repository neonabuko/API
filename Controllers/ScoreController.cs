using Microsoft.AspNetCore.Mvc;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities.Dto;
using Microsoft.EntityFrameworkCore;
using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Controllers;

[ApiController]
[Route("/scores")]
public class ScoreController(ScoreService scoreService) : ControllerBase
{

    [HttpGet("data")]
    public async Task<IActionResult> GetAllDataAsync()
    {
        var scores = await scoreService.GetAllDataAsync();
        return Ok(scores);
    }

    [HttpGet("{name}/data")]
    public async Task<IActionResult> GetDataByNameAsync(string name)
    {
        var score = await scoreService.GetDataByNameAsync(name);
        return Ok(score);
    }

    [HttpGet("{name}")]
    public IActionResult GetFileByNameAsync(string name)
    {
        var score = scoreService.GetFileByNameAsync(name);
        return new FileStreamResult(score, "application/vnd.recordare.musicxml+xml");
    }

    [HttpPost("data")]
    public async Task<IActionResult> SaveDataAsync([FromForm] ScoreDto dto)
    {
        try
        {
            Score score = new()
            {
                Name = dto.Name,
                Title = dto.Title,
                Author = dto.Author ?? "Unknown"
            };
            await scoreService.SaveDataAsync(score);
        }
        catch (DbUpdateException e)
        {
            return StatusCode(409, e.Message);
        }
        return Ok();
    }

    [HttpPost("chunks")]
    public async Task<IActionResult> SaveFileAsync([FromForm] ChunkDto dto)
    {
        await scoreService.SaveFileAsync(dto);
        return StatusCode(202, "Stored chunk " + dto.Id);
    }

    [HttpPost("json")]
    public async Task<IActionResult> SaveJsonAsync(ScoreDto dto)
    {
        try
        {
            Score score = new()
            {
                Name = dto.Name,
                Title = dto.Title,
                Author = dto.Author ?? "Unknown"
            };
            await scoreService.SaveScoreJsonAsync(score, dto.Content);
        }
        catch (DbUpdateException e)
        {
            return StatusCode(409, e.Message);
        }
        return Ok();
    }

    [HttpPatch("data")]
    public async Task<IActionResult> UpdateDataAsync([FromForm] SongEditDto dto)
    {
        Score score = new()
        {
            Name = dto.Name,
            Title = dto.Title,
            Author = dto.Author
        };
        await scoreService.UpdateDataAsync(score);
        return Ok();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteAsync(string name)
    {
        await scoreService.DeleteAsync(name);
        return Ok();
    }
}