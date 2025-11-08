using Application.Shared.Data;

namespace Application.Songs.Errors;

public static class SongErrors
{
    public static readonly Error NotFound = new(
        nameof(NotFound),
        "Song does not exist");
}