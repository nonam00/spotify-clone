using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongList;

public record GetLikedSongListQuery(Guid UserId) : IQuery<LikedSongListVm>;