using Domain.Common;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Models;

// User Aggregate Root
public class User : AggregateRoot<Guid>
{
    public Email Email { get; private init; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public string? FullName { get; private set; }
    public FilePath AvatarPath { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<LikedSong> _userLikedSongs = [];
    private readonly List<Playlist> _playlists = [];
    private readonly List<RefreshToken> _refreshTokens = [];
    private readonly List<Song> _uploadedSongs = [];
    
    public IReadOnlyCollection<Playlist> Playlists => _playlists.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public IReadOnlyCollection<LikedSong> UserLikedSongs => _userLikedSongs.AsReadOnly();
    public IReadOnlyCollection<Song> UploadedSongs => _uploadedSongs.AsReadOnly();
    

    private User() { } // For EF Core
    
    public static User Create(Email email, PasswordHash passwordHash, string? fullName = null)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FullName = fullName?.Trim(),
            AvatarPath = new FilePath(null),
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email));
        return user;
    }

    public void UpdateProfile(string? fullName, FilePath avatarPath)
    {
        FullName = fullName?.Trim();
        var oldAvatarPath = AvatarPath;
        AvatarPath = avatarPath;
        
        if (!string.IsNullOrEmpty(oldAvatarPath) && oldAvatarPath != avatarPath)
        {
            AddDomainEvent(new UserAvatarChangedEvent(Id, newAvatarPath: AvatarPath, oldAvatarPath: oldAvatarPath));
        }
    }

    public void ChangePassword(PasswordHash newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void Activate()
    {
        IsActive = true;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }

    public RefreshToken AddRefreshToken(string token, DateTime expires)
    {
        var refreshToken = RefreshToken.Create(Id, token, expires);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public Song UploadSong(string title, string author, FilePath audioPath, FilePath imagePath)
    {
        var song = Song.Create(
            title: title,
            author: author,
            songPath: audioPath,
            imagePath: imagePath,
            uploaderId: Id);
        
        _uploadedSongs.Add(song);
        
        return song;
    }
    
    public bool LikeSong(Guid songId)
    {
        if (HasLikedSong(songId))
        {
            return false;
        }

        var likedSong = LikedSong.Create(Id, songId);
        _userLikedSongs.Add(likedSong);
        return true;
    }

    public bool UnlikeSong(Guid songId)
    {
        var likedSong = _userLikedSongs.FirstOrDefault(ls => ls.SongId == songId);
        return likedSong != null && _userLikedSongs.Remove(likedSong);
    }

    private bool HasLikedSong(Guid songId) => _userLikedSongs.Any(s => s.SongId == songId);

    public Result<Playlist> CreatePlaylist()
    {
        var title = $"Playlist #{_playlists.Count + 1}";
        var createPlaylistResult = Playlist.Create(userId: Id, title: title);
        
        if (createPlaylistResult.IsFailure)
        {
            return createPlaylistResult;
        }
        
        _playlists.Add(createPlaylistResult.Value);
        
        return createPlaylistResult;
    }
    
    public Playlist? RemovePlaylist(Guid playlistId)
    {
        var playlist = _playlists.FirstOrDefault(p => p.Id == playlistId);
        
        if (playlist != null)
        {
            _playlists.Remove(playlist);
            AddDomainEvent(new PlaylistDeletedEvent(playlistId, playlist.ImagePath));
        }
        
        return playlist;
    }
}