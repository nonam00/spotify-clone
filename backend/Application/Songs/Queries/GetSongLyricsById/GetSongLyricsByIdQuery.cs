using Application.Shared.Messaging;
using Domain.Common;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Songs.Queries.GetSongLyricsById;

public record GetSongLyricsByIdCommand(Guid SongId) : ICommand<Result<IReadOnlyList<LyricsSegmentData>>>;