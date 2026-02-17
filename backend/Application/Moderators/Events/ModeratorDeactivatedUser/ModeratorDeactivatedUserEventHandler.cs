using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

namespace Application.Moderators.Events.ModeratorDeactivatedUser;

public class ModeratorDeactivatedUserEventHandler : IDomainEventHandler<ModeratorDeactivatedUserEvent>
{
    private readonly IFileServiceClient _fileServiceClient;
    private readonly ILogger<ModeratorDeactivatedUserEventHandler> _logger;

    public ModeratorDeactivatedUserEventHandler(
        IFileServiceClient fileServiceClient,
        ILogger<ModeratorDeactivatedUserEventHandler> logger)
    {
        _fileServiceClient = fileServiceClient;
        _logger = logger;
    }

    public Task HandleAsync(ModeratorDeactivatedUserEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling user {UserId} deactivated event.", @event.UserId);

        if (string.IsNullOrWhiteSpace(@event.AvatarPath.Value))
        {
            return Task.CompletedTask;
        }
        
        _logger.LogDebug(
            "Deleting user {UserId} avatar image {AvatarImagePath}.",
            @event.UserId, @event.AvatarPath.Value);

        return _fileServiceClient.DeleteAsync(@event.AvatarPath.Value, "image", cancellationToken);
    }
}