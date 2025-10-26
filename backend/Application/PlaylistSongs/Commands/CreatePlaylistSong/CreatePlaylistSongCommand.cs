using Application.Shared.Messaging;

namespace Application.PlaylistSongs.Commands.CreatePlaylistSong;

public record CreatePlaylistSongCommand(Guid UserId, Guid PlaylistId, Guid SongId) : ICommand<string>;