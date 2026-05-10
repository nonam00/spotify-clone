using Domain.Common;
using Domain.ValueObjects;
using Application.Shared.Messaging;
using Domain.Models;

namespace Application.Songs.Commands.UpdateTranscribeInformation;

public record UpdateTranscribeInformationCommand(
    Guid SongId,
    bool ContainsExplicitContent,
    IReadOnlyList<LyricsSegmentData> LyricsSegments) : ICommand<Result>;