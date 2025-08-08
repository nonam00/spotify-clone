using MediatR;

using Application.LikedSongs.Models;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongList;

public class GetLikedSongListQuery : IRequest<LikedSongListVm>
{
    public Guid UserId { get; init; }
}