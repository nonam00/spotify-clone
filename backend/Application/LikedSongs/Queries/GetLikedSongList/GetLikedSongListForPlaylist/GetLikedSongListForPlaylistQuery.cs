using Application.LikedSongs.Models;
using MediatR;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylist;

public class GetLikedSongListForPlaylistQuery : IRequest<LikedSongListVm>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
}