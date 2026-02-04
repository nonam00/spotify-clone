using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetSongListBySearch;

public class GetSongListBySearchQueryHandler : IQueryHandler<GetSongListBySearchQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetSongListBySearchQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetSongListBySearchQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository
            .GetSearchList(request.SearchString, request.SearchCriteria, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}