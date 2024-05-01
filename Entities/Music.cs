using System.ComponentModel.DataAnnotations;

namespace SongManager.Entities;

public abstract class Music {
    [MaxLength(255)]
    public required string Name { get; set; }
    [MaxLength(255)]
    public required string Title { get; set; }
    [MaxLength(255)]
    public required string Author { get; set; }
}