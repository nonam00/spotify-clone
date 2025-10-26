using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylist;

public record GetLikedSongListForPlaylistQuery(Guid UserId, Guid PlaylistId) : IQuery<LikedSongListVm>;