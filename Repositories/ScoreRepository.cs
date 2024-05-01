using SongManager.Data;
using SongManager.Entities;

namespace Repositories;

public class ScoreRepository(SongManagerContext _context) : MusicRepository<Score>(_context)
{
}