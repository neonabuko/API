using System.ComponentModel.DataAnnotations;

namespace SongManager.Entities;

public class Song
{
    public int Id { get; set; }
    [StringLength(70)]
    public required string Name { get; set; }
    public TimeSpan Duration { get; set; }
    public required string Url { get; set; }
    public string? Author { get; set; }
}