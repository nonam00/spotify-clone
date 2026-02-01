using Microsoft.Extensions.Logging;

using Domain.Common;
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
                "User {UserId} tried update details of playlist {PlaylistId} but playlist does not exist",
                request.UserId, request.PlaylistId);
            return Result.Failure(PlaylistErrors.NotFound);
        }
        
        if (playlist.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User {UserId} tried to update details of playlist {PlaylistId} but playlist belongs to user {OwnerId}",
                request.UserId, request.PlaylistId, playlist.UserId);
            return Result.Failure(PlaylistErrors.OwnershipError);
        }
        
        var title = request.Title;
        var description = request.Description != "" ? request.Description : null;
        var newImagePath = new FilePath(
            !string.IsNullOrWhiteSpace(request.ImagePath)
                ? request.ImagePath
                : playlist.ImagePath.Value);

        var updateResult = playlist.UpdateDetails(title, description, newImagePath);
        if (updateResult.IsFailure)
        {
            _logger.LogInformation(
                "User {UserId} tried to update details of playlist {PlaylistId}" +
                " but domain occurred: {DomainErrorDescription}",
                request.UserId, request.PlaylistId, updateResult.Error.Description);
            return updateResult;
        }
        
        _playlistsRepository.Update(playlist);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}