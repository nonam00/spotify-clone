using Microsoft.Extensions.Logging;

using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Playlists.Errors;
using Application.Playlists.Interfaces;

namespace Application.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommandHandler : ICommandHandler<UpdatePlaylistCommand, Result>
{
    private readonly IPlaylistsRepository _playlistsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePlaylistCommandHandler> _logger;
    
    public UpdatePlaylistCommandHandler(
        IPlaylistsRepository playlistsRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePlaylistCommandHandler> logger)
    {
        _playlistsRepository = playlistsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdatePlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetById(request.PlaylistId, cancellationToken);
        
        if (playlist == null)
        {
            _logger.LogError(
                "User {userId} tried update details of playlist {playlistId} but playlist does not exist",
                request.UserId, request.PlaylistId);
            return Result.Failure(PlaylistErrors.NotFound);
        }
        
        if (playlist.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User {userId} tried to update details of playlist {playlist} but playlist belongs to user {ownerId}",
                request.UserId, request.PlaylistId, playlist.UserId);
            return Result.Failure(PlaylistErrors.OwnershipError);
        }
        
        var title = request.Title;
        var description = request.Description != "" ? request.Description : null;
        var newImagePath = new FilePath(request.ImagePath ?? playlist.ImagePath.Value);

        playlist.UpdateDetails(title, description, newImagePath);
        _playlistsRepository.Update(playlist);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}