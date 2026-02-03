using Domain.Common;

namespace Domain.Errors;

public static class UserDomainErrors
{
    public static readonly Error NotActive =
        new(nameof(NotActive), "User is not active and cannot perform actions.");
    
    public static readonly Error UserAlreadyActive =
        new(nameof(UserAlreadyActive), "User is already active.");
    
    public static readonly Error UserAlreadyDeactivated =
        new(nameof(UserAlreadyDeactivated), "User is already deactivated.");
    
    public static readonly Error RefreshTokenNotFound =
        new (nameof(RefreshTokenNotFound), "Refresh token not found.");
    
    public static readonly Error CannotLikedUnpublishedSong =
        new(nameof(CannotLikedUnpublishedSong), "User cannot like unpublished song.");
    
    public static readonly Error SongAlreadyLiked =
        new(nameof(SongAlreadyLiked), "Song already liked.");
    
    public static readonly Error SongNotLiked =
        new(nameof(SongNotLiked), "Song not liked.");

    public static readonly Error UserDoesNotHavePlaylist =
        new(nameof(UserDoesNotHavePlaylist), "User doesnt have this playlist.");
}