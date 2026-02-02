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

    internal static Result<PlaylistSong> Create(Guid playlistId, Guid songId, int order)
    {
        if (order <= 0)
        {
            return Result<PlaylistSong>.Failure(PlaylistSongDomainErrors.OrderCannotBeLessOrEqualToZero);
        }
        
        var now = DateTime.UtcNow;
        
        return Result<PlaylistSong>.Success(new PlaylistSong
        {
            PlaylistId = playlistId,
            SongId = songId,
            CreatedAt = now,
            UpdatedAt = now,
            Order = order
        });
    }

    internal Result ChangeOrder(int newOrder)
    {
        if (newOrder <= 0)
        {
            return Result.Failure(PlaylistSongDomainErrors.OrderCannotBeLessOrEqualToZero);
        }

        if (Order == newOrder)
        {
            return Result.Failure(PlaylistSongDomainErrors.NewOrderCannotBeEqualToOld);
        }
        
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}

public static class PlaylistSongDomainErrors
{
    public static readonly Error OrderCannotBeLessOrEqualToZero = new(
        nameof(OrderCannotBeLessOrEqualToZero), 
            "Song order in playlist cannot be less than zero or equal to zero");
    
    public static readonly Error NewOrderCannotBeEqualToOld = new(
        nameof(NewOrderCannotBeEqualToOld),
            "New song order in playlist cannot be equal to old order");
}