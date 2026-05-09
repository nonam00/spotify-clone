using Domain.ValueObjects;

namespace Infrastructure.Transcription;

internal static class TranscriptionServiceMessaging
{
    public const string TranscriptionExchange = "transcription-service-exchange";
    public const string TranscribeSongRoutingKey = "transcribe-service.transcribe-song";
    public const string TranscribeSongQueue = "transcribe-service.transcribe-song";
    public const string UpdateSongInformationRoutingKey = "transcribe-service.update-song-information";
    public const string UpdateSongInformationQueue = "transcribe-service.update-song-information";
}

public record TranscribeSongMessage(Guid SongId, string AudioPath);
public record UpdateSongInformationMessage(
    Guid SongId,
    bool ContainsExplicitContent,
    IReadOnlyList<LyricsSegmentData> LyricsSegments);