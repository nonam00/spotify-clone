using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongList;

public class GetLikedSongListQuery : IQuery<LikedSongListVm>
{
    public Guid UserId { get; init; }
}