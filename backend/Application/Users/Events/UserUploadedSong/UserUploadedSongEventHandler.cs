using Domain.Events;
using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Application.Users.Events.UserUploadedSong;

public class UserUploadedSongEventHandler : IDomainEventHandler<UserUploadedSongEvent>
{
    private readonly ITranscriptionServicePublisher _transcriptionServicePublisher;

    public UserUploadedSongEventHandler(ITranscriptionServicePublisher transcriptionServicePublisher)
    {
        _transcriptionServicePublisher = transcriptionServicePublisher;
    }

    public Task HandleAsync(UserUploadedSongEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return _transcriptionServicePublisher.PublishTranscribeSongAsync(
            domainEvent.SongId,
            domainEvent.AudioPath,
            cancellationToken);
    }
}