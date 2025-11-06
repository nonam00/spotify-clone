using Application.Playlists.Models;
using Application.Shared.Messaging;

namespace Application.Playlists.Queries.GetPlaylistListByCount;

public record GetPlaylistListByCountQuery(Guid UserId, int Count) : IQuery<PlaylistListVm>;