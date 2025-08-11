using MediatR;

using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetNewestSongList;

public class GetNewestSongListQueryHandler : IRequestHandler<GetNewestSongListQuery, SongListVm>
{
    private readonly ISongsRepository _songsRepository;

    public GetNewestSongListQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<SongListVm> Handle(GetNewestSongListQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.TakeNewestList(100, cancellationToken);
        return new SongListVm { Songs = songs };
    }
}