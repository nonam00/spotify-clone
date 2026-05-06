using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Infrastructure.Transcription;

public class TranscriptionServicePublisher : ITranscriptionServicePublisher
{
    private readonly IMessagePublisher _publisher;
    
    public TranscriptionServicePublisher(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    public Task PublishTranscribeSongAsync(Guid songId, string audioPath, CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(
            new TranscribeSongMessage(songId, audioPath),
            TranscriptionServiceMessaging.TranscriptionExchange,
            TranscriptionServiceMessaging.TranscribeSongRoutingKey,
            cancellationToken);
    }
}