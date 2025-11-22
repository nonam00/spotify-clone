using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetAllSongs;

public class GetAllSongsQueryHandler : IQueryHandler<GetAllSongsQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetAllSongsQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetAllSongsQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetList(cancellationToken).ConfigureAwait(false);
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}