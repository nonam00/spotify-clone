using MediatR;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetFullLikedSongList
{
    public class GetFullLikedSongListQuery : IRequest<LikedSongListVm>
    {
        public Guid UserId { get; set; }
    }
}
