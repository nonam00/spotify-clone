using Application.LikedSongs.Interfaces;
using MediatR;

namespace Application.LikedSongs.Queries.CheckLikedSong;

public class CheckLikedSongQueryHandler : IRequestHandler<CheckLikedSongQuery, bool>
{
    private readonly ILikedSongsRepository _likedSongsRepository;

    public CheckLikedSongQueryHandler(ILikedSongsRepository likedSongsRepository)
    {
        _likedSongsRepository = likedSongsRepository;
    }

    public async Task<bool> Handle(CheckLikedSongQuery request,
        CancellationToken cancellationToken)
    {
        return await _likedSongsRepository.CheckIfExists(request.UserId, request.SongId, cancellationToken);
    }
}