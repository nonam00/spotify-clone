using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.AddSongToPlaylist;

public record AddSongToPlaylistCommand(Guid UserId, Guid PlaylistId, Guid SongId) : ICommand<Result>;