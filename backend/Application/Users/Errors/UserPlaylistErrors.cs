using Application.Shared.Data;

namespace Application.Users.Errors;

public static class UserPlaylistErrors
{
    public static readonly Error Ownership = new(
        nameof(Ownership),
        "Playlist does not exist or does not belong to you.");
}