using Application.Playlists.Interfaces;
using Application.Playlists.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;

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
        var playlists = await _playlistsRepository.TakeList(
            request.UserId, request.Count, cancellationToken);
        return Result<PlaylistListVm>.Success(new PlaylistListVm(playlists));
    }
}