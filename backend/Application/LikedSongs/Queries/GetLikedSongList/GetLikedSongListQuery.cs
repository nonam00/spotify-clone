using MediatR;

namespace Application.LikedSongs.Queries.GetLikedSongList
{
    public class GetLikedSongListQuery : IRequest<LikedSongListVm>
    {
        public Guid UserId { get; set; }
    }
}
