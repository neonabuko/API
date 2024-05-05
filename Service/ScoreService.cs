using ScoreHubAPI.Entities;
using ScoreHubAPI.Repositories;

namespace ScoreHubAPI.Service;

public class ScoreService(IMusicRepository<Score> scoreRepository, string _scoresPath)
: MusicService<Score>(scoreRepository, _scoresPath)
{
    public async Task SaveScoreJsonAsync(Score score, string content)
    {
        await SaveDataAsync(score);
        await SaveScoreContentAsync(score.Name, content);
    }
}