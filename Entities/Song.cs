namespace SongManager.Entities;

public class Song : Music
{
    public int Id { get; set; }
    public TimeSpan Duration { get; set; }
}