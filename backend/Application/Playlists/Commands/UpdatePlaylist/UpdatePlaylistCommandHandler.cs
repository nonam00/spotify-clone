using MediatR;

using Application.Playlists.Interfaces;

namespace Application.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommandHandler : IRequestHandler<UpdatePlaylistCommand>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public UpdatePlaylistCommandHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task Handle(UpdatePlaylistCommand request, CancellationToken cancellationToken)
    {
        await _playlistsRepository.Update(
            request.PlaylistId,
            request.UserId,
            request.Title,
            request.Description,
            request.ImagePath,
            cancellationToken
        );
    }
}