using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetNewestSongList;

public class GetNewestSongListQueryHandler : IQueryHandler<GetNewestSongListQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetNewestSongListQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetNewestSongListQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository
            .TakeNewestList(100, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}