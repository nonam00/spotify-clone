using MediatR;

using Application.LikedSongs.Models;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylist;

public class GetLikedSongListForPlaylistQuery : IRequest<LikedSongListVm>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
}