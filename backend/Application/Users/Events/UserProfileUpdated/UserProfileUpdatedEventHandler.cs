using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

namespace Application.Users.Events.UserProfileUpdated;

public class UserProfileUpdatedEventHandler : IDomainEventHandler<UserProfileUpdatedEvent>
{ 
    private readonly IFileServiceClient _fileServiceClient;
    private readonly ILogger<UserProfileUpdatedEventHandler> _logger;

    public UserProfileUpdatedEventHandler(IFileServiceClient fileServiceClient, ILogger<UserProfileUpdatedEventHandler> logger)
    {
        _fileServiceClient = fileServiceClient;
        _logger = logger;
    }

    public async Task HandleAsync(UserProfileUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling user {userId} profile updated event", @event.UserId);

        if (@event.OldAvatarPath != @event.NewAvatarPath)
        { 
            _logger.LogDebug("Deleting user {userId} old avatar image {imagePath}",
                @event.UserId, @event.OldAvatarPath.Value);
            await _fileServiceClient.DeleteAsync(@event.OldAvatarPath, cancellationToken).ConfigureAwait(false);
        }
    }
}