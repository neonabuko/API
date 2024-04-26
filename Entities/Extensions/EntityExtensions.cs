using SongManager.Entities.Dto;

namespace SongManager.Entities.Extensions;

public static class EntityExtensions
{
    public static SongViewDto AsViewDto(this Song song) {
        return new SongViewDto(
            song.Name,
            song.Author,
            song.Duration,
            song.Bitrate
        );
    }

}
