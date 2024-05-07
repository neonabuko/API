using ScoreHubAPI.Entities;
using ScoreHubAPI.Repositories;

namespace ScoreHubAPI.Service;

public class ScoreService(IMusicRepository<Score> scoreRepository, string _scoresPath)
: MusicService<Score>(scoreRepository, _scoresPath)
{
    public async Task SaveFromJsonAsync(Score score, string content)
    {
        await SaveDataAsync(score);
        await WriteFromJsonAsync(score.Name, content);
    }
}