using MediatR;

using Application.Playlists.Interfaces;

namespace Application.Playlists.Commands.DeletePlaylist;

public class DeletePlaylistCommandHandler : IRequestHandler<DeletePlaylistCommand, string?>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public DeletePlaylistCommandHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<string?> Handle(DeletePlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetById(request.PlaylistId, request.UserId, cancellationToken);
        await _playlistsRepository.Delete(playlist, cancellationToken);
        return playlist.ImagePath;
    }
}