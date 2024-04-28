using System.ComponentModel.DataAnnotations;

namespace SongManager.Entities;

public class Score
{
    public int Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }
    [MaxLength(255)]
    public string? Author { get; set; }
}