using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Infrastructure.Email;

public class EmailServicePublisher : IEmailServicePublisher
{
    private readonly IMessagePublisher _publisher;
    
    public EmailServicePublisher(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    public Task PublishSendConfirmEmail(string email, string code, CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(
            new SendEmailMessage(email, code, EmailTopicType.Confirm),
            EmailServiceContract.EmailExchange,
            EmailServiceContract.SendEmailRoutingKey,
            cancellationToken);
    }
    
    public Task PublishSendRestoreEmail(string email, string code, CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(
            new SendEmailMessage(email, code, EmailTopicType.Restore),
            EmailServiceContract.EmailExchange,
            EmailServiceContract.SendEmailRoutingKey,
            cancellationToken);
    }
}