using MediatR;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListBySearchStringAndPlaylistId
{
    public class GetLikedSongListBySearchStringAndPlaylistIdQuery
        : IRequest<LikedSongListVm>
    {
        public Guid UserId { get; init; }
        public Guid PlaylistId { get; init; }
        public string SearchString { get; init; } = null!;
    }
}
