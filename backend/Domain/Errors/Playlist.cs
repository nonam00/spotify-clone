using Domain.Common;

namespace Domain.Errors;

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

    public static readonly Error SongOrderCannotBeLessOrEqualToZero = new(
        nameof(SongOrderCannotBeLessOrEqualToZero), 
        "Song order in playlist cannot be less than zero or equal to zero.");

    public static readonly Error NewSongOrderCannotBeEqualToOld = new(
        nameof(NewSongOrderCannotBeEqualToOld),
        "New song order in playlist cannot be equal to old order.");
}