using MediatR;

using Application.LikedSongs.Models;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylistBySearch;

public class GetLikedSongListForPlaylistBySearchQuery : IRequest<LikedSongListVm>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
    public string SearchString { get; init; } = null!;
}