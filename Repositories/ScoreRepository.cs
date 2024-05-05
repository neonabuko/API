using ScoreHubAPI.Data;
using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Repositories;

public class ScoreRepository(ScoreHubContext _context) : MusicRepository<Score>(_context)
{
}