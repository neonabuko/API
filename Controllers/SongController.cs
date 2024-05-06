using Microsoft.AspNetCore.Mvc;
using ScoreHubAPI.Service;
using ScoreHubAPI.Entities.Dto;
using ScoreHubAPI.Entities.Extensions;
using ScoreHubAPI.Entities;
using Microsoft.EntityFrameworkCore;
using ScoreHubAPI.Rules;

namespace ScoreHubAPI.Controllers;

[ApiController]
[Route("/songs")]
public class SongController(SongService songService, SongRules songRules) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllDataAsync()
    {
        var songs = await songService.GetAllDataAsync();
        var songsAsDto = songs.Select(s => s.AsDto()).ToList();
        return Ok(songsAsDto);
    }

    [HttpGet("{name}/data")]
    public async Task<IActionResult> GetDataByNameAsync(string name)
    {
        var song = await songService.GetDataByNameAsync(name);
        return Ok(song.AsDto());
    }

    [HttpGet("{name}")]
    public IActionResult Stream(string name) => songService.Stream(Request, Response, name);    

    [HttpPost("data")]
    public async Task<IActionResult> SaveDataAsync([FromForm] SongDto dto)
    {
        Song song = new()
        {
            Name = dto.Name,
            Title = dto.Title,
            Author = dto.Author ?? "Unknown",
            Duration = dto.Duration
        };
        try
        {
            await songRules.HandleSave(song);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        catch (DbUpdateException e)
        {
            return Conflict(e.Message);
        }

        await songService.SaveDataAsync(song);
        return Ok();
    }

    [HttpPost("chunks")]
    public async Task<IActionResult> SaveFileAsync([FromForm] ChunkDto dto)
    {
        await songService.SaveFileAsync(dto);
        return Ok();
    }


    [HttpPatch("data")]
    public async Task<IActionResult> UpdateDataAsync([FromForm] MusicEditDto dto)
    {
        await songService.UpdateDataAsync(dto);
        return Ok();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteAsync(string name)
    {
        await songService.DeleteAsync(name);
        return Ok();
    }
}

