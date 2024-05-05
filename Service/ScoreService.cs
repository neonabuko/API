using ScoreHubAPI.Entities;
using ScoreHubAPI.Entities.Dto;
using ScoreHubAPI.Entities.Extensions;
using ScoreHubAPI.Repositories;

namespace ScoreHubAPI.Service;

public class ScoreService(IConfiguration configuration, ScoreRepository scoreRepository)
{
    private readonly string _scoresPath = configuration.GetValue<string>("ScoresPath") ?? throw new NullReferenceException();
    private readonly ChunkService chunkService = new("/app/scores");
    
    public async Task<ICollection<ScoreViewDto>> GetAllScoreDataAsync()
    {
        var scores = await scoreRepository.GetAllAsync();
        return scores.Select(scores => scores.AsViewDto()).ToList();
    }

    public async Task<ScoreViewDto> GetScoreDataByNameAsync(string name)
    {
        var score = await scoreRepository.GetByNameAsync(name);
        return score.AsViewDto();
    }

    public FileStream GetScoreFileByNameAsync(string name)
    {
        string scorePath = Path.Combine(_scoresPath, name);
        var fileStream = new FileStream(scorePath, FileMode.Open, FileAccess.Read);
        return fileStream;
    }

    public async Task SaveScoreDataAsync(ScoreDto scoreDto)
    {
        Score score = new()
        {
            Name = scoreDto.Name,
            Title = scoreDto.Title,
            Author = scoreDto.Author ?? "Unknown"
        };

        await scoreRepository.CreateAsync(score);
    }

    public async Task SaveScoreFileAsync(ChunkDto chunkDto)
    {
        try
        {
            await scoreRepository.GetByNameAsync(chunkDto.Name);
            throw new InvalidOperationException("Score already exists.");
        }
        catch (NullReferenceException) { }

        await chunkService.StoreChunkAsync(chunkDto.Name, chunkDto.Id, chunkDto.TotalChunks, chunkDto.Data);
        if (await chunkService.IsFileCompleteAsync(chunkDto.Name, chunkDto.TotalChunks))
        {
            await chunkService.ReconstructFileAsync(chunkDto.Name, chunkDto.TotalChunks);
        }
    }

    public async Task SaveScoreAsync(ScoreDto scoreDto)
    {
        try
        {
            await scoreRepository.GetByNameAsync(scoreDto.Name);
            throw new InvalidOperationException("Score already exists.");
        }
        catch (NullReferenceException) { }

        await chunkService.StoreScoreContentAsync(scoreDto.Name, scoreDto.Content);

        Score score = new()
        {
            Name = scoreDto.Name,
            Title = scoreDto.Title,
            Author = scoreDto.Author ?? "Unknown"
        };

        await scoreRepository.CreateAsync(score);
    }

    public async Task UpdateDataAsync(SongEditDto songEditDto)
    {
        await scoreRepository.UpdateAsync(songEditDto.Name, songEditDto.Title ?? "", songEditDto.Author ?? "");
    }

    public async Task DeleteScoreDataAsync(string name)
    {
        await scoreRepository.DeleteAsync(name);
        var scorePath = Path.Combine($"/app/scores/{name}");
        if (File.Exists(scorePath))
        {
            File.Delete(scorePath);
        }
    }

}