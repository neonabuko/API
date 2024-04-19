using SongManager.Entities;

namespace SongManager;

public static class EntityExtensions
{
    public static SongDto AsDto(this Song song)
    {
        return new SongDto(
            Name: song.Name,
            Duration: song.Duration,
            Url: song.Url,
            Author: song.Author ?? ""
        );
    }

}
