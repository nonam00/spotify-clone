using MediatR;

using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetAllSongs;

public class GetAllSongsQueryHandler : IRequestHandler<GetAllSongsQuery, SongListVm>
{
    private readonly ISongsRepository _songsRepository;

    public GetAllSongsQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<SongListVm> Handle(GetAllSongsQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetList(cancellationToken);
        return new SongListVm { Songs = songs };
    }
}