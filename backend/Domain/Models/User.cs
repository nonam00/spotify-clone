using Domain.Common;
using Domain.Errors;
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

    public Result UpdateProfile(string? fullName, FilePath avatarPath)
    {
        if (!IsActive)
        {
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        FullName = fullName?.Trim();
        var oldAvatarPath = AvatarPath;
        AvatarPath = avatarPath;
        
        if (!string.IsNullOrEmpty(oldAvatarPath) && oldAvatarPath != avatarPath)
        {
            AddDomainEvent(new UserAvatarChangedEvent(Id, oldAvatarPath: oldAvatarPath));
        }

        return Result.Success();
    }

    public Result ChangePassword(PasswordHash newPasswordHash)
    {
        if (!IsActive)
        {
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        PasswordHash = newPasswordHash;
        
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(UserDomainErrors.UserAlreadyActive);
        }
        
        IsActive = true;
        
        return Result.Success();
    }
    
    internal Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure(UserDomainErrors.UserAlreadyDeactivated);
        }
        
        IsActive = false;
        AvatarPath = new FilePath("");
        
        return Result.Success();
    }

    public Result<RefreshToken> AddRefreshToken()
    {
        if (!IsActive)
        {
            return Result<RefreshToken>.Failure(UserDomainErrors.NotActive);
        }
        
        var refreshToken = RefreshToken.Create(Id);
        _refreshTokens.Add(refreshToken);
        
        return Result<RefreshToken>.Success(refreshToken);
    }

    public Result<RefreshToken> UpdateRefreshToken(string refreshTokenValue)
    {
        if (!IsActive)
        {
            return Result<RefreshToken>.Failure(UserDomainErrors.NotActive);
        }
        
        var refreshToken = _refreshTokens.FirstOrDefault(rf => rf.Token == refreshTokenValue);
        
        if (refreshToken == null || !_refreshTokens.Remove(refreshToken))
        {
            return Result<RefreshToken>.Failure(UserDomainErrors.RefreshTokenNotFound);
        }
        
        refreshToken.UpdateToken();
        
        _refreshTokens.Add(refreshToken);
        
        return Result<RefreshToken>.Success(refreshToken);
    }

    public Result<Song> UploadSong(string title, string author, FilePath audioPath, FilePath imagePath)
    {
        if (!IsActive)
        {
            return Result<Song>.Failure(UserDomainErrors.NotActive);
        }
        
        var createSongResult = Song.Create(
            title: title,
            author: author,
            songPath: audioPath,
            imagePath: imagePath,
            uploaderId: Id);

        if (createSongResult.IsFailure)
        {
            return Result<Song>.Failure(createSongResult.Error);
        }
        
        _uploadedSongs.Add(createSongResult.Value);
        
        return Result<Song>.Success(createSongResult.Value);
    }
    
    public Result LikeSong(Song song)
    {
        if (!IsActive)
        {
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        if (!song.IsPublished)
        {
            return Result.Failure(UserDomainErrors.CannotLikedUnpublishedSong);
        }
        
        if (HasLikedSong(song.Id))
        {
            return Result.Failure(UserDomainErrors.SongAlreadyLiked);
        }

        var likedSong = LikedSong.Create(Id, song.Id);
        _userLikedSongs.Add(likedSong);
        return Result.Success();
    }

    public Result UnlikeSong(Guid songId)
    {
        if (!IsActive)
        {
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var likedSong = _userLikedSongs.FirstOrDefault(ls => ls.SongId == songId);
        if (likedSong is null || !_userLikedSongs.Remove(likedSong))
        {
            return  Result.Failure(UserDomainErrors.SongNotLiked);
        } 
        return Result.Success();
    }

    private bool HasLikedSong(Guid songId) => _userLikedSongs.Any(s => s.SongId == songId);

    public Result<Playlist> CreatePlaylist()
    {
        if (!IsActive)
        {
            return Result<Playlist>.Failure(UserDomainErrors.NotActive);
        }
        
        var title = $"Playlist #{_playlists.Count + 1}";
        var createPlaylistResult = Playlist.Create(userId: Id, title: title);
        
        if (createPlaylistResult.IsFailure)
        {
            return createPlaylistResult;
        }
        
        _playlists.Add(createPlaylistResult.Value);
        
        return createPlaylistResult;
    }
    
    public Result RemovePlaylist(Guid playlistId)
    {
        if (!IsActive)
        {
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var playlist = _playlists.FirstOrDefault(p => p.Id == playlistId);

        if (playlist is null || !_playlists.Remove(playlist))
        {
            return Result.Failure(UserDomainErrors.UserDoesNotHavePlaylist);
        }

        AddDomainEvent(new PlaylistDeletedEvent(playlistId, playlist.ImagePath));
        
        return Result.Success();
    }

    public Result CleanRefreshTokens()
    {
        if (!IsActive)
        {
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        _refreshTokens.Clear();
        return Result.Success();
    }
}