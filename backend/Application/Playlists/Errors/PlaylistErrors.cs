using Domain.Common;

namespace Application.Playlists.Errors;

public static class PlaylistErrors
{
    public static readonly Error NotFound = 
        new(nameof(NotFound), "Playlist does not exist.");

    public static readonly Error OwnershipError = 
        new(nameof(OwnershipError), "You do not have permissions to this playlist.");
}