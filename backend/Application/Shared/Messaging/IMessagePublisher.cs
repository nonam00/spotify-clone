namespace Application.Shared.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default);
}