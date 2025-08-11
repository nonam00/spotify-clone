using MediatR;

using Application.Playlists.Interfaces;

namespace Application.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommandHandler : IRequestHandler<UpdatePlaylistCommand, string?>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public UpdatePlaylistCommandHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<string?> Handle(UpdatePlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetById(request.PlaylistId, request.UserId, cancellationToken);
        
        playlist.Title = request.Title;
        playlist.Description = request.Description != "" ? request.Description : null;
        playlist.CreatedAt = DateTime.UtcNow;

        string? oldImagePath = null;
        if (request.ImagePath is not null)
        {
            oldImagePath = playlist.ImagePath;
            playlist.ImagePath = request.ImagePath;
        }

        await _playlistsRepository.Update(playlist, cancellationToken);

        return oldImagePath;
    }
}