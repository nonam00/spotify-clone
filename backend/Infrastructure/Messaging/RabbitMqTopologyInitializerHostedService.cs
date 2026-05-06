using Microsoft.Extensions.Hosting;

namespace Infrastructure.Messaging;

public class RabbitMqTopologyInitializedHostedService : IHostedService
{
    private readonly RabbitMqTopologyInitializer _initializer;

    public RabbitMqTopologyInitializedHostedService(RabbitMqTopologyInitializer initializer)
    {
        _initializer = initializer;
    }

    public Task StartAsync(CancellationToken cancellationToken) => _initializer.InitializeAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}