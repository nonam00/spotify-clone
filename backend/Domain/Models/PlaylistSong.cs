using Domain.Common;
using Domain.Errors;

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
            return Result<PlaylistSong>.Failure(PlaylistDomainErrors.SongOrderCannotBeLessOrEqualToZero);
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
            return Result.Failure(PlaylistDomainErrors.SongOrderCannotBeLessOrEqualToZero);
        }

        if (Order == newOrder)
        {
            return Result.Failure(PlaylistDomainErrors.NewSongOrderCannotBeEqualToOld);
        }
        
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}