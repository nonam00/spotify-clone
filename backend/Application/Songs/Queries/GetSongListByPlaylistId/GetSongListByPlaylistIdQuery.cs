using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongListByPlaylistId;

public record GetSongListByPlaylistIdQuery(Guid PlaylistId) : IQuery<SongListVm>;