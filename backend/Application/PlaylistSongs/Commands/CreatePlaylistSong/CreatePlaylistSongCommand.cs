using Application.Shared.Messaging;

namespace Application.PlaylistSongs.Commands.CreatePlaylistSong;

public class CreatePlaylistSongCommand : ICommand<string>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
    public Guid SongId { get; init; }
}