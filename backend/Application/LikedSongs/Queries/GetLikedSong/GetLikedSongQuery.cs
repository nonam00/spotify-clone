using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.LikedSongs.Queries.GetLikedSong
{
    public class GetLikedSongQuery : IRequest<LikedSongVm?>
    {
        public Guid UserId { get; set; }
        public Guid SongId { get; set; }
    }
}
