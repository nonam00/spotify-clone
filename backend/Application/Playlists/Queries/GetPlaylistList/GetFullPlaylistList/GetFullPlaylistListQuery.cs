using Application.Shared.Messaging;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistList.GetFullPlaylistList;

public record GetFullPlaylistListQuery(Guid UserId) : IQuery<PlaylistListVm>;