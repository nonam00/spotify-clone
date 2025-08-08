using MediatR;

using Domain;
using Application.Playlists.Interfaces;

namespace Application.Playlists.Commands.CreatePlaylist;

public class CreatePlaylistCommandHandler : IRequestHandler<CreatePlaylistCommand, Guid>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public CreatePlaylistCommandHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<Guid> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
    {
        var count = await _playlistsRepository.GetCount(request.UserId, cancellationToken);

        var playlist = new Playlist
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = $"Playlist #{count + 1}"
        };

        await _playlistsRepository.Add(playlist, cancellationToken);
            
        return playlist.Id;
    }
}