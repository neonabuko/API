using ScoreHubAPI.Repositories;
using ScoreHubAPI.Entities;

namespace ScoreHubAPI.Service;

public class SongService(IMusicRepository<Song> songRepository, string _songsPath) 
: MusicService<Song>(songRepository, _songsPath)
{
    
}


