using Application.Shared.Messaging;

using Domain;
using Application.Playlists.Interfaces;
using Application.Users.Interfaces;

namespace Application.Playlists.Commands.CreatePlaylist;

public class CreatePlaylistCommandHandler : ICommandHandler<CreatePlaylistCommand, Guid>
{
    private readonly IPlaylistsRepository _playlistsRepository;
    private readonly IUsersRepository _usersRepository;

    public CreatePlaylistCommandHandler(IPlaylistsRepository playlistsRepository, IUsersRepository usersRepository)
    {
        _playlistsRepository = playlistsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<Guid> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
    {
        var isUserActive = await _usersRepository.CheckIfActivated(request.UserId, cancellationToken);
        
        if (!isUserActive)
        {
            throw new Exception("Account is not activated. User can not perform any actions");
        }
        
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