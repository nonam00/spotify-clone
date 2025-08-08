using MediatR;

using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongById;

public class GetSongByIdQueryHandler : IRequestHandler<GetSongByIdQuery, SongVm>
{
    private readonly ISongsRepository _songsRepository;

    public GetSongByIdQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<SongVm> Handle(GetSongByIdQuery request, CancellationToken cancellationToken)
    {
        var song = await _songsRepository.GetById(request.SongId, cancellationToken);
        return song;
    }
}