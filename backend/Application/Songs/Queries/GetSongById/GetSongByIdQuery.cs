using MediatR;

using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongById;

public class GetSongByIdQuery : IRequest<SongVm>
{
    public Guid SongId { get; init; }
}