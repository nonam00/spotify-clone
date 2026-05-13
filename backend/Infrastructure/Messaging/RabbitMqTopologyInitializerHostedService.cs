using Microsoft.Extensions.Hosting;

namespace Infrastructure.Messaging;

public sealed class RabbitMqTopologyInitializerHostedService : IHostedService
{
    private readonly RabbitMqTopologyInitializer _initializer;

    public RabbitMqTopologyInitializerHostedService(RabbitMqTopologyInitializer initializer)
    {
        _initializer = initializer;
    }

    public Task StartAsync(CancellationToken cancellationToken) => _initializer.InitializeAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}