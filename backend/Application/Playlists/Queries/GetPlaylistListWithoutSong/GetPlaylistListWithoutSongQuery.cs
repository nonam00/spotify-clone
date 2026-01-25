using Application.Playlists.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Playlists.Queries.GetPlaylistListWithoutSong;

public record GetPlaylistListWithoutSongQuery(Guid UserId, Guid SongId) : IQuery<Result<PlaylistListVm>>;