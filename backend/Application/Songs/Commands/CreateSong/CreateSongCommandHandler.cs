using Application.Shared.Messaging;

using Domain;
using Application.Songs.Interfaces;

namespace Application.Songs.Commands.CreateSong;

public class CreateSongCommandHandler : ICommandHandler<CreateSongCommand, Guid>
{
    private readonly ISongsRepository _songsRepository;

    public CreateSongCommandHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Guid> Handle(CreateSongCommand request,
        CancellationToken cancellationToken)
    {
        var song = new Song
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Author = request.Author,
            SongPath = request.SongPath,
            ImagePath = request.ImagePath
        };

        await _songsRepository.Add(song, cancellationToken);

        return song.Id;
    }
}