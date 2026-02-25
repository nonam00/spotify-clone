namespace Application.Shared.Integration;

public interface IEmailServicePublisher
{
    Task PublishSendConfirmEmail(string email, string code, CancellationToken cancellationToken = default);
    Task PublishSendRestoreEmail(string email, string code, CancellationToken cancellationToken = default);
}