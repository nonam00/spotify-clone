using Application.LikedSongs.Interfaces;
using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongList;

public class GetLikedSongListQueryHandler : IQueryHandler<GetLikedSongListQuery, LikedSongListVm>
{
    private readonly ILikedSongsRepository _likedSongsRepository;

    public GetLikedSongListQueryHandler(ILikedSongsRepository likedSongsRepository)
    {
        _likedSongsRepository = likedSongsRepository;
    }

    public async Task<LikedSongListVm> Handle(GetLikedSongListQuery request,
        CancellationToken cancellationToken)
    {
        var liked = await _likedSongsRepository.GetList(request.UserId, cancellationToken);

        return new LikedSongListVm(liked);
    }
}