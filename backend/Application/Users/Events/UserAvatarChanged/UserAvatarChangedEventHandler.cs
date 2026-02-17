using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

namespace Application.Users.Events.UserAvatarChanged;

public class UserAvatarChangedEventHandler : IDomainEventHandler<UserAvatarChangedEvent>
{ 
    private readonly IFileServiceClient _fileServiceClient;
    private readonly ILogger<UserAvatarChangedEventHandler> _logger;

    public UserAvatarChangedEventHandler(
        IFileServiceClient fileServiceClient,
        ILogger<UserAvatarChangedEventHandler> logger)
    {
        _fileServiceClient = fileServiceClient;
        _logger = logger;
    }

    public Task HandleAsync(UserAvatarChangedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling user {userId} profile updated event", @event.UserId);
        
        _logger.LogDebug("Deleting user {userId} old avatar image {imagePath}",
            @event.UserId, @event.OldAvatarPath.Value);

        return _fileServiceClient.DeleteAsync(@event.OldAvatarPath, "image", cancellationToken);
    }
}