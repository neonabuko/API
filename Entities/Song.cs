namespace SongManager.Entities;

public class Song
{
    public int Id { get; set; }
    public required byte[] File { get; set; }
}