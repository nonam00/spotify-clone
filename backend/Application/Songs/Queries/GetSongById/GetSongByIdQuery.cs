using MediatR;

namespace Application.Songs.Queries.GetSongById
{
    public class GetSongByIdQuery : IRequest<SongVm>
    {
        public Guid SongId { get; init; }
    }
}
