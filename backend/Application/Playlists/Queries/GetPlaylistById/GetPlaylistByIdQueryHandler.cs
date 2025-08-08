using MediatR;

using Application.Playlists.Interfaces;
using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQueryHandler : IRequestHandler<GetPlaylistByIdQuery, PlaylistVm>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public GetPlaylistByIdQueryHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<PlaylistVm> Handle(GetPlaylistByIdQuery request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetById(request.Id, cancellationToken)
                       ?? throw new Exception("Playlist this such ID doesn't exist");

        var playlistVm = new PlaylistVm
        {
            Id = playlist.Id,
            Title = playlist.Title,
            Description = playlist.Description,
            ImagePath = playlist.ImagePath
        };
        
        return playlistVm;
    }
}