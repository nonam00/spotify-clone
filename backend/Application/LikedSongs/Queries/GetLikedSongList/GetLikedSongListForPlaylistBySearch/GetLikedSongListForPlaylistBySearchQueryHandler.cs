using Application.LikedSongs.Interfaces;
using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylistBySearch;

public class GetLikedSongListForPlaylistBySearchQueryHandler
    : IQueryHandler<GetLikedSongListForPlaylistBySearchQuery, LikedSongListVm>
{
    private readonly ILikedSongsRepository _likedSongsRepository;

    public GetLikedSongListForPlaylistBySearchQueryHandler(ILikedSongsRepository likedSongsRepository)
    {
        _likedSongsRepository = likedSongsRepository;
    }

    public async Task<LikedSongListVm> Handle(
        GetLikedSongListForPlaylistBySearchQuery request,
        CancellationToken cancellationToken)
    {
        var liked = await _likedSongsRepository.GetSearchListForPlaylist(
            request.UserId, request.SearchString, request.PlaylistId, cancellationToken);
        
        return new LikedSongListVm(liked);
    }
}