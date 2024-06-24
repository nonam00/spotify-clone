using MediatR;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListBySearchStringAndPlaylistId
{
    public class GetLikedSongListBySearchStringAndPlaylistIdQuery
        : IRequest<LikedSongListVm>
    {
        public Guid UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public string SearchString { get; set; } = null!;
    }
}
