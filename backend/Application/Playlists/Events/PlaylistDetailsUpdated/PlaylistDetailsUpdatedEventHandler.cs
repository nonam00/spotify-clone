using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Messaging;

namespace Application.Playlists.Events.PlaylistDetailsUpdated;

public class PlaylistDetailsUpdatedEventHandler : IDomainEventHandler<PlaylistDetailsUpdatedEvent>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlaylistDetailsUpdatedEventHandler> _logger;

    public PlaylistDetailsUpdatedEventHandler(ILogger<PlaylistDetailsUpdatedEventHandler> logger, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task HandleAsync(PlaylistDetailsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Playlist details updated: {PlaylistId}", @event.PlaylistId);

        if (@event.OldImagePath != @event.NewImagePath)
        {
            await _httpClient.DeleteAsync(
                "http://nginx/files/api/v1?type=image&file_id=" + @event.OldImagePath,
                cancellationToken);
        }
    }
}