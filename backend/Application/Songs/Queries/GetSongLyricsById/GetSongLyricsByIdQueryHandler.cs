using Domain.Common;
using Domain.ValueObjects;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;

namespace Application.Songs.Queries.GetSongLyricsById;

public class GetSongLyricsByIdQueryHandler : ICommandHandler<GetSongLyricsByIdQuery, Result<List<LyricsSegmentData>>>
{
    private readonly ISongsRepository _songsRepository;

    public GetSongLyricsByIdQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<List<LyricsSegmentData>>> Handle(
        GetSongLyricsByIdQuery query, CancellationToken cancellationToken)
    {
        var lyrics = await _songsRepository
            .GetLyricsBySongId(query.SongId, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<List<LyricsSegmentData>>.Success(lyrics);
    }
}