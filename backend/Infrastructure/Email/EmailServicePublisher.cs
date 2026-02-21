using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Infrastructure.Email;

public class EmailServicePublisher : IEmailServicePublisher, IAsyncDisposable
{
    private readonly IMessagePublisher _publisher;
    
    public EmailServicePublisher(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    public Task PublishSendConfirmEmail(string email, string code, CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(
            new SendEmailMessage(email, code),
            EmailServiceContract.EmailExchange,
            EmailServiceContract.SendConfirmEmailRoutingKey,
            cancellationToken);
    }
    
    public Task PublishSendRestoreEmail(string email, string code, CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(
            new SendEmailMessage(email, code),
            EmailServiceContract.EmailExchange,
            EmailServiceContract.SendRestoreEmailRoutingKey,
            cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_publisher is IAsyncDisposable disposablePublisher)
        {
            await disposablePublisher.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}
