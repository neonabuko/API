using ScoreHubAPI.Entities;
using ScoreHubAPI.Entities.Dto;
using ScoreHubAPI.Entities.Extensions;
using ScoreHubAPI.Repositories;

namespace ScoreHubAPI.Service;

public class ScoreService(IConfiguration configuration, ScoreRepository scoreRepository)
{
    private readonly string _scoresPath = configuration.GetValue<string>("ScoresPath") ?? throw new NullReferenceException();
    private readonly ChunkService chunkService = new("/app/scores");
    
    public async Task<ICollection<ScoreViewDto>> GetAllDataAsync()
    {
        var scores = await scoreRepository.GetAllAsync();
        return scores.Select(scores => scores.AsViewDto()).ToList();
    }

    public async Task<ScoreViewDto> GetDataByNameAsync(string name)
    {
        var score = await scoreRepository.GetByNameAsync(name);
        return score.AsViewDto();
    }

    public FileStream GetFileByNameAsync(string name)
    {
        string path = Path.Combine(_scoresPath, name);
        var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return fileStream;
    }

    public async Task SaveDataAsync(ScoreDto dto)
    {
        Score score = new()
        {
            Name = dto.Name,
            Title = dto.Title,
            Author = dto.Author ?? "Unknown"
        };

        await scoreRepository.CreateAsync(score);
    }

    public async Task SaveFileAsync(ChunkDto dto)
    {
        await chunkService.StoreChunkAsync(dto.Name, dto.Id, dto.TotalChunks, dto.Data);
        if (await chunkService.IsFileCompleteAsync(dto.Name, dto.TotalChunks))
        {
            await chunkService.ReconstructFileAsync(dto.Name, dto.TotalChunks);
        }
    }

    public async Task SaveScoreAsync(ScoreDto dto)
    {
        Score score = new()
        {
            Name = dto.Name,
            Title = dto.Title,
            Author = dto.Author ?? "Unknown"
        };
        await scoreRepository.CreateAsync(score);

        await chunkService.StoreScoreContentAsync(dto.Name, dto.Content);
    }

    public async Task UpdateDataAsync(SongEditDto dto)
    {
        await scoreRepository.UpdateAsync(dto.Name, dto.Title ?? "", dto.Author ?? "");
    }

    public async Task DeleteDataAsync(string name)
    {
        await scoreRepository.DeleteAsync(name);
        var path = Path.Combine($"/app/scores/{name}");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

}