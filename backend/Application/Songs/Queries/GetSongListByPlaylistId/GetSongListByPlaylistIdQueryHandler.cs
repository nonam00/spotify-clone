using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongListByPlaylistId;

public class GetSongListByPlaylistIdQueryHandler: IQueryHandler<GetSongListByPlaylistIdQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetSongListByPlaylistIdQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetSongListByPlaylistIdQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetListByPlaylistId(request.PlaylistId, cancellationToken);
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}