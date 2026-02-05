using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Tests.Helpers;

public static class PlaylistHelpers
{
    public static Playlist CreatePlaylist(Guid userId, bool withImage = false)
    {
        var imagePath = withImage
            ? new FilePath("playlist.png")
            : new FilePath(null);

        return Playlist.Create(
            userId: userId,
            title: "Playlist",
            description: "Description",
            imagePath: imagePath).Value;
    }
}