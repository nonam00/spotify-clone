using Application.Playlists.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Queries.GetPlaylistListWithoutSong;

public record GetPlaylistListWithoutSongQuery(Guid UserId, Guid SongId) : IQuery<Result<PlaylistListVm>>;