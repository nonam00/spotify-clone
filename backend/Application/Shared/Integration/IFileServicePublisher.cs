namespace Application.Shared.Integration;

public interface IFileServicePublisher
{
    Task PublishDeleteFileAsync(string id, string fileType, CancellationToken cancellationToken = default);
}