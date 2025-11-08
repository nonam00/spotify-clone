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
    
    public static Playlist Create(Guid userId, string title, string? description = null, FilePath? imagePath = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }

        return new Playlist
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title.Trim(),
            Description = description?.Trim(),
            ImagePath = imagePath ?? new FilePath(null),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string title, string? description, FilePath imagePath)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var oldImagePath = ImagePath;
        
        Title = title.Trim();
        Description = description?.Trim();
        ImagePath = imagePath;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PlaylistDetailsUpdatedEvent(
            Id,
            UserId, 
            title,
            description!,
            imagePath,
            oldImagePath));
    }
    
    public bool AddSong(Guid songId)
    {
        if (ContainsSong(songId))
        {
            return false;
        }

        var playlistSong = PlaylistSong.Create(Id, songId);
        _playlistSongs.Add(playlistSong);
        
        UpdatedAt = DateTime.UtcNow;
        
        return true;
    }

    public bool RemoveSong(Guid songId)
    {
        var playlistSong = _playlistSongs.FirstOrDefault(ps => ps.SongId == songId);
        UpdatedAt = DateTime.UtcNow;
        return playlistSong != null && _playlistSongs.Remove(playlistSong);
    }

    private bool ContainsSong(Guid songId) => _playlistSongs.Any(ps => ps.SongId == songId);
}