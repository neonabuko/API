using ScoreHubAPI.Data;
using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Repositories;

public class SongRepository(ScoreHubContext _context) : MusicRepository<Song>(_context)
{
}