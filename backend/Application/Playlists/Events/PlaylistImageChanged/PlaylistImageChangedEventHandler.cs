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
        _logger.LogDebug("Handling playlist {PlaylistId} details updated event",  @event.PlaylistId);

        if (!string.IsNullOrWhiteSpace(@event.OldImagePath))
        {
            _logger.LogDebug(
                "Deleting playlist {PlaylistId} old cover image {ImagePath}",
                @event.PlaylistId, @event.OldImagePath);
            
            await _fileServiceClient
                .DeleteAsync(@event.OldImagePath, "image", cancellationToken)
                .ConfigureAwait(false);
        }
    }
}