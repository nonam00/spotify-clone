using Domain.Common;

namespace Domain.Models;

public class PlaylistSong : Entity
{
    public Guid PlaylistId { get; private set; }
    public Guid SongId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
        
    // EF Core navigation properties
    public Playlist Playlist { get; private set; } = null!;
    public Song Song { get; private set; } = null!;

    private PlaylistSong() { }

    public static PlaylistSong Create(Guid playlistId, Guid songId)
    {
        var now = DateTime.UtcNow;
        return new PlaylistSong
        {
            PlaylistId = playlistId,
            SongId = songId,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void UpdateTimestamps(DateTime currentTime)
    {
        UpdatedAt = currentTime;
    }
}