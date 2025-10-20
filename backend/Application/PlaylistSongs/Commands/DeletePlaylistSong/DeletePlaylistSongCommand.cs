using Application.Shared.Messaging;

namespace Application.PlaylistSongs.Commands.DeletePlaylistSong;

public class DeletePlaylistSongCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
    public Guid SongId { get; init; }
}