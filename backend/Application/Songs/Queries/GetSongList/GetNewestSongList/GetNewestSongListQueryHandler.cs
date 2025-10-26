using Application.Shared.Messaging;

using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetNewestSongList;

public class GetNewestSongListQueryHandler : IQueryHandler<GetNewestSongListQuery, SongListVm>
{
    private readonly ISongsRepository _songsRepository;

    public GetNewestSongListQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<SongListVm> Handle(GetNewestSongListQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.TakeNewestList(100, cancellationToken);
        return new SongListVm(songs);
    }
}