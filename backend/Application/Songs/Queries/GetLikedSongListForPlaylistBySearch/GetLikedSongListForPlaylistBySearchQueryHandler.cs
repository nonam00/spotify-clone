using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetLikedSongListForPlaylistBySearch;

public class GetLikedSongListForPlaylistBySearchQueryHandler 
    : IQueryHandler<GetLikedSongListForPlaylistBySearchQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetLikedSongListForPlaylistBySearchQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetLikedSongListForPlaylistBySearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchedLikedNotInPlaylist = await _songsRepository.GetSearchLikedByUserIdExcludeInPlaylist(
            request.UserId, request.PlaylistId, request.SearchString, cancellationToken);
        return Result<SongListVm>.Success(new SongListVm(searchedLikedNotInPlaylist));
    }
}