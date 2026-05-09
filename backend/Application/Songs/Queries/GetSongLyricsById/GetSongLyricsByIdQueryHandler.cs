using Domain.Common;
using Domain.ValueObjects;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;

namespace Application.Songs.Queries.GetSongLyricsById;

public class GetSongLyricsByIdCommandHandler : ICommandHandler<GetSongLyricsByIdQuery, Result<IReadOnlyList<LyricsSegmentData>>>
{
    private readonly ISongsRepository _songsRepository;

    public GetSongLyricsByIdCommandHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<IReadOnlyList<LyricsSegmentData>>> Handle(
        GetSongLyricsByIdQuery query, CancellationToken cancellationToken)
    {
        var lyrics = await _songsRepository
            .GetLyricsBySongId(query.SongId, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<IReadOnlyList<LyricsSegmentData>>.Success(lyrics);
    }
}