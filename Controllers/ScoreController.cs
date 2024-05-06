using Microsoft.AspNetCore.Mvc;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities.Dto;
using Microsoft.EntityFrameworkCore;
using ScoreHubAPI.Entities;
using ScoreHubAPI.Rules;

namespace ScoreHubAPI.Controllers;

[ApiController]
[Route("/scores")]
public class ScoreController(ScoreService scoreService, ScoreRules scoreRules) : ControllerBase
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
        Score score = new()
        {
            Name = dto.Name,
            Title = dto.Title,
            Author = dto.Author ?? "Unknown"
        };

        await scoreRules.HandleSave(score);
        await scoreService.SaveDataAsync(score);
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
        Score score = new()
        {
            Name = dto.Name,
            Title = dto.Title,
            Author = dto.Author ?? "Unknown"
        };
        try
        {
            await scoreService.SaveJsonAsync(
                score,
                dto.Content ?? throw new ArgumentNullException(nameof(dto), "Must provide content")
            );
        }
        catch (DbUpdateException e)
        {
            return Conflict(e.Message);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        return Ok();
    }

    [HttpPatch("data")]
    public async Task<IActionResult> UpdateDataAsync([FromForm] MusicEditDto dto)
    {
        await scoreService.UpdateDataAsync(dto);
        return Ok();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteAsync(string name)
    {
        await scoreService.DeleteAsync(name);
        return Ok();
    }
}