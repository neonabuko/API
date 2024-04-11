using Microsoft.AspNetCore.Mvc;
using SongManager.Entities;

namespace SongManager;

public static class EntityExtensions
{
    // public static SongDto AsDto(Song song)
    // {
    //     MemoryStream memoryStream = new(song.File);
    //     FileStreamResult fileStreamResult = new(memoryStream, "audio/mpeg");
    //     return new SongDto(
    //         fileStreamResult
    //     );
    // }

}
