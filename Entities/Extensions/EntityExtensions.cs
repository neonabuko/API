using Microsoft.AspNetCore.Mvc;
using SongManager.Entities;

namespace SongManager;

public static class EntityExtensions
{
    public static SongViewDto AsViewDto(this Song song) {
        return new SongViewDto(
            song.Name,
            song.Author,
            song.Duration
        );
    }

}
