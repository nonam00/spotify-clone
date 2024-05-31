using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Songs.Queries.GetSongById
{
    public class GetSongByIdQuery : IRequest<SongVm>
    {
        public Guid SongId { get; set; }
    }
}
