using Domain.Common;
using Domain.ValueObjects;
using Application.Shared.Messaging;

namespace Application.Songs.Queries.GetSongLyricsById;

public record GetSongLyricsByIdQuery(Guid SongId) : ICommand<Result<List<LyricsSegmentData>>>;