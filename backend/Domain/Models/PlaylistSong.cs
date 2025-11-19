using Domain.Common;

namespace Domain.Models;

public class PlaylistSong : Entity
{
    public Guid PlaylistId { get; private set; }
    public Guid SongId { get; private set; }
    public int Order { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
        
    // EF Core navigation properties
    public Playlist Playlist { get; private set; } = null!;
    public Song Song { get; private set; } = null!;

    private PlaylistSong() { }

    public static PlaylistSong Create(Guid playlistId, Guid songId, int order)
    {
        if (order <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, "Order should be greater than 0");
        }
        var now = DateTime.UtcNow;
        return new PlaylistSong
        {
            PlaylistId = playlistId,
            SongId = songId,
            CreatedAt = now,
            UpdatedAt = now,
            Order = order
        };
    }

    public void ChangeOrder(int newOrder)
    {
        if (newOrder <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newOrder), newOrder, "Order should be greater than 0");
        }

        if (Order == newOrder)
        {
            throw new InvalidOperationException("Tried to pass current order");
        }
        
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}