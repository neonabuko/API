using Microsoft.AspNetCore.Mvc;
using Repositories;
using SongManager.Entities;
using SongManager.Entities.Dto;
using SongManager.Entities.Extensions;

namespace SongManager.Controllers;

[ApiController]
public class ScoreController(ScoreRepository scoreRepository) : ControllerBase
{
    [HttpGet("/scores")]
    public async Task<IActionResult> GetAllAsync()
    {
        var scores = await scoreRepository.GetAllAsync();
        return Ok(scores);
    }

    [HttpGet("/score/${name}")]
    public async Task<IActionResult> GetAsync(string name)
    {
        var score = await scoreRepository.GetByNameAsync(name);
        return Ok(score.AsDto());
    }

    [HttpPost("/scores")]
    public async Task<IActionResult> SaveAsync(ScoreDto scoreDto)
    {
        Score score = new()
        {
            Name = scoreDto.Name,
            Author = scoreDto.Author
        };

        await scoreRepository.CreateAsync(score);
        return Ok();
    }

    [HttpDelete("/scores/${name}")]
    public async Task<IActionResult> DeleteAsync(string name)
    {
        await scoreRepository.DeleteAsync(name);
        return Ok();
    }
}