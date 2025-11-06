using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetLikedSongListForPlaylistBySearch;

public class GetLikedSongListForPlaylistBySearchQueryHandler 
    : IQueryHandler<GetLikedSongListForPlaylistBySearchQuery, SongListVm>
{
    private readonly ISongsRepository _songsRepository;

    public GetLikedSongListForPlaylistBySearchQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<SongListVm> Handle(GetLikedSongListForPlaylistBySearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchedLiked = await _songsRepository.GetSearchLikedByUserId(
            request.UserId, request.SearchString, cancellationToken);
        var songsInPlaylist = await _songsRepository.GetListByPlaylistId(
            request.PlaylistId, cancellationToken);
        var songs = searchedLiked.Except(songsInPlaylist).ToList();
        return new SongListVm(songs);
    }
}