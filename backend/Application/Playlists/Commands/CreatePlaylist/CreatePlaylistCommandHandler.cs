using Application.Shared.Messaging;

using Domain;
using Application.Playlists.Interfaces;
using Application.Users.Interfaces;

namespace Application.Playlists.Commands.CreatePlaylist;

public class CreatePlaylistCommandHandler : ICommandHandler<CreatePlaylistCommand, Guid>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public CreatePlaylistCommandHandler(IPlaylistsRepository playlistsRepository, IUsersRepository usersRepository)
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