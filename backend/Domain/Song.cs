namespace Domain;

public class Song
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = null!;
    public string SongPath { get; set; } = null!;
    public string ImagePath { get; set; } = null!;
    public string Author { get; set; } = null!;
    public Guid? UserId { get; set; }
    public User? User { get; set; }
}