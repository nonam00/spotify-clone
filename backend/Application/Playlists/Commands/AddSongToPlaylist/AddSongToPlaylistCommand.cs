using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Commands.AddSongToPlaylist;

public record AddSongToPlaylistCommand(Guid UserId, Guid PlaylistId, Guid SongId) : ICommand<Result>;