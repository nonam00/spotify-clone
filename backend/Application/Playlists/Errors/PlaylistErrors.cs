using Domain.Common;

namespace Application.Playlists.Errors;

public static class PlaylistErrors
{
    public static readonly Error NotFound = new(
        nameof(NotFound), 
        "Playlist does not exist.");

    public static readonly Error OwnershipError = new(
        nameof(OwnershipError), 
        "You do not have permissions to this playlist.");
    
    public static readonly Error SongAlreadyInPlaylist = new(
        nameof(SongAlreadyInPlaylist), 
        "Song already added to playlist.");
    
    public static readonly Error SongNotInPlaylist = new(
        nameof(SongNotInPlaylist),
        "Song was not added to playlist.");
}