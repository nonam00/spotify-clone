using Domain.Common;
using Application.Shared.Messaging;
using Application.Songs.Enums;
using Application.Songs.Interfaces;
using Application.Songs.Models;

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
        var searchInLyrics = request is { SearchCriteria: SearchCriteria.Any, SearchString.Length: >= 20 };
        var songs = await _songsRepository
            .GetSearchList(
                request.SearchString,
                request.SearchCriteria,
                searchInLyrics,
                cancellationToken)
            .ConfigureAwait(false);
        
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}