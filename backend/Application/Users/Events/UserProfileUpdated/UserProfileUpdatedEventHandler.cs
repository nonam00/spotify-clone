using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Messaging;

namespace Application.Users.Events.UserProfileUpdated;

public class UserProfileUpdatedEventHandler : IDomainEventHandler<UserProfileUpdatedEvent>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserProfileUpdatedEventHandler> _logger;

    public UserProfileUpdatedEventHandler(HttpClient httpClient, ILogger<UserProfileUpdatedEventHandler> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task HandleAsync(UserProfileUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("User profile updated: {UserId}", @event.UserId);

        if (@event.OldAvatarPath != @event.NewAvatarPath)
        {
            await _httpClient.DeleteAsync(
                "http://nginx/files/api/v1?type=image&file_id=" + @event.OldAvatarPath,
                cancellationToken);
        }
    }
}