using Domain.Events;
using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Application.Moderators.Events.ModeratorDeactivatedUser;

public class ModeratorDeactivatedUserEventHandler : IDomainEventHandler<ModeratorDeactivatedUserEvent>
{
    private readonly IFileServicePublisher _fileServicePublisher;
    
    public ModeratorDeactivatedUserEventHandler(IFileServicePublisher fileServicePublisher)
    {
        _fileServicePublisher = fileServicePublisher;
    }

    public Task HandleAsync(ModeratorDeactivatedUserEvent @event, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(@event.AvatarPath))
        {
            return Task.CompletedTask;
        }
        return _fileServicePublisher.PublishDeleteFileAsync(@event.AvatarPath, "image", cancellationToken);
    }
}