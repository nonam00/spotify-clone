using Application.Shared.Messaging;

using Application.Playlists.Interfaces;
using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistList.GetFullPlaylistList;

public class GetPlaylistListByUserIdQueryHandler : IQueryHandler<GetFullPlaylistListQuery, PlaylistListVm>
{
    private readonly IPlaylistsRepository _playlistsRepository;
    public GetPlaylistListByUserIdQueryHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<PlaylistListVm> Handle(
        GetFullPlaylistListQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _playlistsRepository.GetList(request.UserId, cancellationToken);

        return new PlaylistListVm(list);
    }
}