using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

namespace Application.Playlists.Events.PlaylistImageChanged;

public class PlaylistImageChangedEventHandler : IDomainEventHandler<PlaylistImageChangedEvent>
{ 
    private readonly IFileServiceClient _fileServiceClient;
    private readonly ILogger<PlaylistImageChangedEventHandler> _logger;

    public PlaylistImageChangedEventHandler(
        IFileServiceClient fileServiceClient,
        ILogger<PlaylistImageChangedEventHandler> logger)
    {
        _fileServiceClient = fileServiceClient;
        _logger = logger;
    }

    public async Task HandleAsync(PlaylistImageChangedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling playlist {playlistId} details updated event",  @event.PlaylistId);
        
        _logger.LogDebug("Deleting playlist {playlistId} old cover image {imagePath}",
                @event.PlaylistId, @event.OldImagePath.Value);
            
        await _fileServiceClient
            .DeleteAsync(@event.OldImagePath, "image", cancellationToken)
            .ConfigureAwait(false);
    }
}