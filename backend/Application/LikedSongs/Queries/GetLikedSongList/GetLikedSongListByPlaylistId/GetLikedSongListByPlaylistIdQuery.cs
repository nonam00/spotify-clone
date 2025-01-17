using MediatR;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListByPlaylistId
{
    public class GetLikedSongListByPlaylistIdQuery : IRequest<LikedSongListVm>
    {
        public Guid UserId { get; init; }
        public Guid PlaylistId { get; init; }
    }
}
