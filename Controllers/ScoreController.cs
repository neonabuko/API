using Microsoft.AspNetCore.Mvc;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities.Dto;

namespace ScoreHubAPI.Controllers;

[ApiController]
[Route("/scores")]
public class ScoreController(ScoreService scoreService) : ControllerBase
{

    [HttpGet("data")]
    public async Task<IActionResult> GetAllScoreDataAsync()
    {
        var scores = await scoreService.GetAllScoreDataAsync();
        return Ok(scores);
    }

    [HttpGet("{name}/data")]
    public async Task<IActionResult> GetScoreDataByNameAsync(string name)
    {
        var score = await scoreService.GetScoreDataByNameAsync(name);
        return Ok(score);
    }

    [HttpGet("{name}")]
    public IActionResult GetScoreFileByNameAsync(string name)
    {
        var score = scoreService.GetScoreFileByNameAsync(name);
        return new FileStreamResult(score, "application/vnd.recordare.musicxml+xml");
    }

    [HttpPost("data")]
    public async Task<IActionResult> SaveScoreDataAsync([FromForm] ScoreDto scoreDto)
    {
        await scoreService.SaveScoreDataAsync(scoreDto);
        return Ok();
    }

    [HttpPost("chunks")]
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

    [HttpPost("json")]
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

    [HttpPatch("data")]
    public async Task<IActionResult> UpdateDataAsync([FromForm] SongEditDto songEditDto)
    {
        await scoreService.UpdateDataAsync(songEditDto);
        return Ok();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteScoreDataAsync(string name)
    {
        await scoreService.DeleteScoreDataAsync(name);
        return Ok();
    }
}