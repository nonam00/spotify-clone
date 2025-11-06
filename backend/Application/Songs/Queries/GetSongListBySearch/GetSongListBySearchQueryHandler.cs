using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongListBySearch;

public class GetSongListBySearchQueryHandler : IQueryHandler<GetSongListBySearchQuery, SongListVm>
{
    private readonly ISongsRepository _songsRepository;

    public GetSongListBySearchQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<SongListVm> Handle(GetSongListBySearchQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetSearchList(
            request.SearchString, request.SearchCriteria, cancellationToken);
        return new SongListVm(songs);
    }
}