using Application.Playlists.Interfaces;
using Application.Playlists.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Queries.GetPlaylistListByCount;

public class GetPlaylistListByCountQueryHandler : IQueryHandler<GetPlaylistListByCountQuery, Result<PlaylistListVm>>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public GetPlaylistListByCountQueryHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<Result<PlaylistListVm>> Handle(GetPlaylistListByCountQuery request, CancellationToken cancellationToken)
    {
        var playlists = await _playlistsRepository
            .TakeList(request.UserId, request.Count, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<PlaylistListVm>.Success(new PlaylistListVm(playlists));
    }
}