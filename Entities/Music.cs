using System.ComponentModel.DataAnnotations;

namespace ScoreHubAPI.Entities;

public abstract class Music {
    public int Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }
    [MaxLength(255)]
    public required string Title { get; set; }
    [MaxLength(255)]
    public required string Author { get; set; }
    public DateTime PublishedAt { get; set; }
}