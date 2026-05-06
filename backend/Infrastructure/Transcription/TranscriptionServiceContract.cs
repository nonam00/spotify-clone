namespace Infrastructure.Transcription;

public class TranscriptionServiceMessaging
{
    public const string TranscribeExchange = "transcribe-service-exchange";
    public const string TranscribeSongRoutingKey = "transcribe-service.transcribe-song";
    public const string TranscribeSongQueue = "transcribe-service.transcribe-song";
    public const string UpdateSongInformationRoutingKey = "transcribe-service.update-song-information";
    public const string UpdateSongInformationQueue = "transcribe-service.update-song-information";
}

public record TranscribeSongMessage(Guid SongId, string SongPath);
public record UpdateSongInformationMessage(Guid SongId, bool ContainsExplicitContent);