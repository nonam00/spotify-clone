using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongListByPlaylistId;

public class GetSongListByPlaylistIdQueryHandler: IQueryHandler<GetSongListByPlaylistIdQuery, SongListVm>
{
    private readonly ISongsRepository _songsRepository;

    public GetSongListByPlaylistIdQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<SongListVm> Handle(GetSongListByPlaylistIdQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetListByPlaylistId(request.PlaylistId, cancellationToken);
        return new SongListVm(songs);
    }
}