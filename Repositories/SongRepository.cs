using SongManager.Data;
using SongManager.Entities;

namespace Repositories;

public class SongRepository(SongManagerContext _context) : MusicRepository<Song>(_context)
{
}