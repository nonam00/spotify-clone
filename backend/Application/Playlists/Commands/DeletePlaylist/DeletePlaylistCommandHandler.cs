using MediatR;

using Application.Playlists.Interfaces;

namespace Application.Playlists.Commands.DeletePlaylist;

public class DeletePlaylistCommandHandler : IRequestHandler<DeletePlaylistCommand>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public DeletePlaylistCommandHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task Handle(DeletePlaylistCommand request,
        CancellationToken cancellationToken)
    {
        await _playlistsRepository.Delete(request.PlaylistId, request.UserId, cancellationToken);
    }
}