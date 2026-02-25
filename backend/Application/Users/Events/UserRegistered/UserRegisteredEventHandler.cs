using Domain.Events;
using Domain.ValueObjects;
using Application.Shared.Integration;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Events.UserRegistered;

public class UserRegisteredEventHandler : IDomainEventHandler<UserRegisteredEvent>
{
    private readonly IEmailServicePublisher _emailServicePublisher;
    private readonly ICodesRepository _codesRepository;

    public UserRegisteredEventHandler(IEmailServicePublisher emailServicePublisher, ICodesRepository codesRepository)
    {
        _emailServicePublisher = emailServicePublisher;
        _codesRepository = codesRepository;
    }

    public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken = default)
    {
        var code = new UserCode();
        
        await _codesRepository
            .SetConfirmationCode(@event.Email, code, code.CodeExpiry)
            .ConfigureAwait(false);
        
        await _emailServicePublisher
            .PublishSendConfirmEmail(@event.Email, code, cancellationToken)
            .ConfigureAwait(false);
    }
}