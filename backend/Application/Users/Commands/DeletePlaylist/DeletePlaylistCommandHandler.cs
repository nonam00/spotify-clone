using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Models;

namespace Application.Users.Commands.DeletePlaylist;

public class DeletePlaylistCommandHandler : ICommandHandler<DeletePlaylistCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePlaylistCommandHandler> _logger;
    
    public DeletePlaylistCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeletePlaylistCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeletePlaylistCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithPlaylists(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError(
                "Tried to delete playlist {PlaylistId} but user {UserId} doesn't exist", 
                request.PlaylistId, request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogError(
                "User {UserId} tried to delete playlist {PlaylistId} but user is not active.",
                request.UserId, request.PlaylistId);
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var removePlaylistResult = user.RemovePlaylist(request.PlaylistId);
        if (removePlaylistResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to delete playlist {PlaylistId}" +
                " but domain error occurred: {DomainErrorDescription}",
                request.PlaylistId, request.UserId, removePlaylistResult.Error.Description);
            return removePlaylistResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "User {UserId} successfully deleted playlist {PlaylistId}", 
            request.UserId, request.PlaylistId);
        
        return Result.Success();
    }
}