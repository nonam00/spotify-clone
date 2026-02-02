using Domain.Common;

namespace Application.Songs.Errors;

public static class SongErrors
{
    public static readonly Error NotFound =
        new(nameof(NotFound), "Song does not exist");
    
    public static readonly Error SongsNotFound =
        new(nameof(SongsNotFound), "Songs not found");
    
    public static readonly Error SomeSongsNotFound =
        new(nameof(SomeSongsNotFound), "Some songs cannot be found");
}