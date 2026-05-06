namespace Application.Shared.Integration;

public interface ITranscriptionServicePublisher
{
    Task PublishTranscribeSongAsync(Guid songId, string audioPath, CancellationToken cancellationToken = default);
}