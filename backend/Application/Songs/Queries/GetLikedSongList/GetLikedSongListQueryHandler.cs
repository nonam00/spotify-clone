using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetLikedSongList;

public class GetLikedSongListQueryHandler : IQueryHandler<GetLikedSongListQuery, SongListVm>
{
    private readonly ISongsRepository _songsRepository;

    public GetLikedSongListQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<SongListVm> Handle(GetLikedSongListQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetLikedByUserId(request.UserId, cancellationToken);
        return new SongListVm(songs);
    }
}