using Microsoft.AspNetCore.Mvc;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities.Dto;

namespace ScoreHubAPI.Controllers;

[ApiController]
public class ScoreController(ScoreService scoreService) : ControllerBase
{

    [HttpGet("/scores/data")]
    public async Task<IActionResult> GetAllScoreDataAsync()
    {
        var scores = await scoreService.GetAllScoreDataAsync();
        return Ok(scores);
    }

    [HttpGet("/scores/{name}/data")]
    public async Task<IActionResult> GetScoreDataByNameAsync(string name)
    {
        var score = await scoreService.GetScoreDataByNameAsync(name);
        return Ok(score);
    }

    [HttpGet("/scores/{name}")]
    public IActionResult GetScoreFileByNameAsync(string name)
    {
        var score = scoreService.GetScoreFileByNameAsync(name);
        return new FileStreamResult(score, "application/vnd.recordare.musicxml+xml");
    }

    [HttpPost("/scores/data")]
    public async Task<IActionResult> SaveScoreDataAsync([FromForm] ScoreDto scoreDto)
    {
        await scoreService.SaveScoreDataAsync(scoreDto);
        return Ok();
    }

    [HttpPost("/scores/chunks")]
    public async Task<IActionResult> SaveScoreFileAsync([FromForm] ChunkDto chunkDto)
    {
        try
        {
            await scoreService.SaveScoreFileAsync(chunkDto);
        }
        catch (InvalidOperationException e)
        {
            return StatusCode(409, e.Message);
        }
        return StatusCode(202, "Stored chunk " + chunkDto.Id);
    }

    [HttpPost("/scores/json")]
    public async Task<IActionResult> SaveScoreAsync(ScoreDto scoreDto)
    {
        try
        {
            await scoreService.SaveScoreAsync(scoreDto);
        }
        catch (InvalidOperationException e)
        {
            return StatusCode(409, e.Message);
        }
        return Ok();
    }

    [HttpPatch("/scores/data")]
    public async Task<IActionResult> UpdateDataAsync([FromForm] SongEditDto songEditDto)
    {
        await scoreService.UpdateDataAsync(songEditDto);
        return Ok();
    }

    [HttpDelete("/scores/{name}")]
    public async Task<IActionResult> DeleteScoreDataAsync(string name)
    {
        await scoreService.DeleteScoreDataAsync(name);
        return Ok();
    }
}