using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Infrastructure.Files;

public class FileServicePublisher : IFileServicePublisher, IAsyncDisposable
{
    private readonly IMessagePublisher _publisher;
    
    public FileServicePublisher(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    public Task PublishDeleteFileAsync(string id, string fileType, CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(
            new DeleteFileMessage(fileType, id),
            FileServiceMessaging.FileExchange,
            FileServiceMessaging.DeleteRoutingKey,
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