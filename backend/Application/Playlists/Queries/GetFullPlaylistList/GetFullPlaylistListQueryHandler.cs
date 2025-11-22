using Application.Playlists.Interfaces;
using Application.Playlists.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Playlists.Queries.GetFullPlaylistList;

public class GetPlaylistListByUserIdQueryHandler : IQueryHandler<GetFullPlaylistListQuery, Result<PlaylistListVm>>
{
    private readonly IPlaylistsRepository _playlistsRepository;
    
    public GetPlaylistListByUserIdQueryHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<Result<PlaylistListVm>> Handle(GetFullPlaylistListQuery request, CancellationToken cancellationToken)
    {
        var list = await _playlistsRepository.GetList(request.UserId, cancellationToken).ConfigureAwait(false);
        return Result<PlaylistListVm>.Success(new PlaylistListVm(list));
    }
}