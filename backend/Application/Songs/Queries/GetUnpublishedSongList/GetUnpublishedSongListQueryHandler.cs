using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetUnpublishedSongList;

public class GetUnpublishedSongListQueryHandler : IQueryHandler<GetUnpublishedSongListQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetUnpublishedSongListQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetUnpublishedSongListQuery query, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetUnpublishedList(cancellationToken).ConfigureAwait(false);
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}