using System.ComponentModel.DataAnnotations;

namespace SongManager.Entities;

public class Song
{
    public int Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Author { get; set; }
}