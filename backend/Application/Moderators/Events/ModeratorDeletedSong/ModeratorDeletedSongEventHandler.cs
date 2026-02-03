using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

namespace Application.Moderators.Events.ModeratorDeletedSong;

public class ModeratorDeletedSongEventHandler : IDomainEventHandler<ModeratorDeletedSongEvent>
{
    private readonly IFileServiceClient _fileServiceClient;
    private readonly ILogger<ModeratorDeletedSongEventHandler> _logger;

    public ModeratorDeletedSongEventHandler(
        IFileServiceClient fileServiceClient,
        ILogger<ModeratorDeletedSongEventHandler> logger)
    {
        _fileServiceClient = fileServiceClient;
        _logger = logger;
    }

    public async Task HandleAsync(ModeratorDeletedSongEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling song {SongId} deleted event", @event.SongId);
        
        _logger.LogDebug("Deleting song {SongId} image {ImagePath}", @event.SongId, @event.Image.Value);
        var deleteImageTask = _fileServiceClient.DeleteAsync(@event.Image, "image", cancellationToken);
        
        _logger.LogDebug("Deleting song {SongId} audio {AudioPath}", @event.SongId, @event.Audio.Value);
        var deleteAudioTask = _fileServiceClient.DeleteAsync(@event.Audio, "audio", cancellationToken);
        
        await Task.WhenAll(deleteImageTask, deleteAudioTask).ConfigureAwait(false);
    }
}