using Domain.Common;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Models;

// Playlist Aggregate Root
public class Playlist : AggregateRoot<Guid>
{
    public Guid UserId { get; private init; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public FilePath ImagePath { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core navigation property        
    public virtual User User { get; private set; } = null!;
    
    // Private collection for tracking many-to-many relationship with metadata
    private readonly List<PlaylistSong> _playlistSongs = [];
    public IReadOnlyCollection<PlaylistSong> PlaylistSongs => _playlistSongs.AsReadOnly();   
    
    private Playlist() { } // For EF Core
    
    public static Result<Playlist> Create(
        Guid userId, string title, string? description = null, FilePath? imagePath = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Playlist>.Failure(PlaylistDomainErrors.EmptyTitle);
        }

        return Result<Playlist>.Success(new Playlist
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title.Trim(),
            Description = description?.Trim(),
            ImagePath = imagePath ?? new FilePath(null),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public Result UpdateDetails(string title, string? description, FilePath imagePath)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(PlaylistDomainErrors.EmptyTitle);
        }

        var oldImagePath = ImagePath;
        
        Title = title.Trim();
        Description = description?.Trim();
        ImagePath = imagePath;
        UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != imagePath)
        {
            AddDomainEvent(new PlaylistImageChangedEvent(Id, oldImagePath));
        }
        
        return Result.Success();
    }
    
    public Result AddSong(Song song)
    {
        if (!song.IsPublished)
        {
            return Result.Failure(PlaylistDomainErrors.CannotPerformActionsWithUnpublishedSong);
        }
        
        if (ContainsSong(song.Id))
        {
            return Result.Failure(PlaylistDomainErrors.AlreadyContainsSong);
        }

        var createPlaylistSongResult = PlaylistSong.Create(Id, song.Id, _playlistSongs.Count + 1);

        if (createPlaylistSongResult.IsFailure)
        {
            return Result.Failure(createPlaylistSongResult.Error);
        }
        
        _playlistSongs.Add(createPlaylistSongResult.Value);
        
        UpdatedAt = DateTime.UtcNow;
        
        return Result.Success();
    }

    public Result AddSongs(List<Song> songs)
    {
        if (ContainsSongs(songs.Select(s => s.Id)))
        {
            return Result.Failure(PlaylistDomainErrors.AlreadyContainsSong);
        }

        if (songs.Any(song => !song.IsPublished))
        {
            return Result.Failure(PlaylistDomainErrors.CannotPerformActionsWithUnpublishedSong);
        }

        foreach (var song in songs)
        {
            var createPlaylistSongResult = PlaylistSong.Create(Id, song.Id, _playlistSongs.Count + 1);

            if (createPlaylistSongResult.IsFailure)
            {
                return Result.Failure(createPlaylistSongResult.Error);
            }
        
            _playlistSongs.Add(createPlaylistSongResult.Value);
        }
        
        UpdatedAt = DateTime.UtcNow;
        
        return Result.Success();
    }

    public Result RemoveSong(Guid songId)
    {
        var playlistSong = _playlistSongs.FirstOrDefault(ps => ps.SongId == songId);

        if (playlistSong == null || !_playlistSongs.Remove(playlistSong))
        {
            return Result.Failure(PlaylistDomainErrors.DoesntContainSong);
        }
        
        UpdatedAt = DateTime.UtcNow;
        
        return Result.Success();
    }
    
    // Song id index in list dictates what order it will get
    public Result ReorderSongs(List<Guid> songsToReorder)
    {
        for (var i = 0; i < _playlistSongs.Count; i++)
        {
            var songId = songsToReorder[i];
            
            var playlistSong = _playlistSongs.FirstOrDefault(ps => ps.SongId == songId);

            if (playlistSong == null)
            {
                return Result.Failure(PlaylistDomainErrors.DoesntContainSong);
            }

            if (playlistSong.Order != i + 1)
            {
                var changeOrderResult = playlistSong.ChangeOrder(i + 1);
                if (changeOrderResult.IsFailure)
                {
                    return changeOrderResult;
                }
            }
        }
        
        return Result.Success();
    }

    private bool ContainsSong(Guid songId) => _playlistSongs.Any(ps => ps.SongId == songId);
    private bool ContainsSongs(IEnumerable<Guid> songIds) => _playlistSongs.Any(ps => songIds.Contains(ps.SongId));
}

public static class PlaylistDomainErrors
{
    public static readonly Error EmptyTitle =
        new(nameof(EmptyTitle), "Playlist title cannot be empty.");
    
    public static readonly Error AlreadyContainsSong =
        new(nameof(AlreadyContainsSong), "Playlist already contains song.");
    
    public static readonly Error DoesntContainSong =
        new(nameof(DoesntContainSong), "Playlist does not contain song.");
    
    public static readonly Error CannotPerformActionsWithUnpublishedSong = new(
        nameof(CannotPerformActionsWithUnpublishedSong),
        "Trying to do action with unpublished song.");
}
