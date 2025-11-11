using Domain.Common;

namespace Domain.Models;

public class LikedSong : Entity
{
    public Guid UserId { get; private set; }
    public Guid SongId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // EF Core navigation properties
    public User User { get; private set; } = null!;
    public Song Song { get; private set; } = null!;
    
    private LikedSong() { }

    public static LikedSong Create(Guid userId, Guid songId)
    {
        return new LikedSong
        {
            UserId = userId,
            SongId = songId,
            CreatedAt = DateTime.UtcNow
        };
    }
}