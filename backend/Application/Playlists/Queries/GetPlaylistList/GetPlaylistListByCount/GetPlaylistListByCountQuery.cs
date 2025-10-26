using Application.Shared.Messaging;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistList.GetPlaylistListByCount;

public record GetPlaylistListByCountQuery(Guid UserId, int Count) : IQuery<PlaylistListVm>;