using Microsoft.AspNetCore.Mvc;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities.Dto;
using ScoreHubAPI.Rules;
using ScoreHubAPI.Entities.Extensions;

namespace ScoreHubAPI.Controllers;

[ApiController]
[Route("/scores")]
public class ScoreController(ScoreService scoreService, ScoreRules scoreRules) : ControllerBase
{

    [HttpGet("data")]
    public async Task<IActionResult> GetAllDataAsync()
    {
        var scores = await scoreService.GetAllDataAsync();
        var scoresAsDto = scores.Select(s => s.AsViewDto());
        return Ok(scoresAsDto);
    }

    [HttpGet("{name}/data")]
    public async Task<IActionResult> GetDataByNameAsync(string name)
    {
        var score = await scoreService.GetDataByNameAsync(name);
        return Ok(score.AsViewDto());
    }

    [HttpGet("{id:int}/data")]
    public async Task<IActionResult> GetDataByIdAsync(int id)
    {
        var score = await scoreService.GetDataByIdAsync(id);
        return Ok(score.AsViewDto());
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetFileByNameAsync(string name)
    {
        var score = await scoreService.GetFileByNameAsync(name);
        return new FileStreamResult(score, "application/vnd.recordare.musicxml+xml");
    }

    [HttpPost("data")]
    public async Task<IActionResult> SaveDataAsync([FromForm] ScoreDto dto)
    {
        var score = dto.AsScore();
        scoreRules.HandleSaveAsync(score);
        var scoreId = await scoreService.SaveDataAsync(score);
        return Ok(scoreId);
    }

    [HttpPost("chunks")]
    public async Task<IActionResult> SaveFileAsync([FromForm] ChunkDto dto)
    {
        await scoreService.SaveFileAsync(dto);
        return Ok();
    }

    [HttpPost("json")]
    public async Task<IActionResult> SaveFromJsonAsync(ScoreDto dto)
    {
        var score = dto.AsScore();
        scoreRules.HandleSaveAsync(score);
        if (dto.Content == null) return BadRequest("Must provide content.");
        await scoreService.SaveFromJsonAsync(score, dto.Content);
        return Ok();
    }

    [HttpPatch("data")]
    public async Task<IActionResult> UpdateDataAsync([FromForm] MusicEditDto dto)
    {
        scoreRules.HandleUpdateData(dto);
        await scoreService.UpdateDataAsync(dto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await scoreService.DeleteAsync(id);
        return Ok();
    }
}