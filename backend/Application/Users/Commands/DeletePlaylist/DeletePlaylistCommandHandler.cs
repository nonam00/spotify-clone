using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

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

        if (user == null)
        {
            _logger.LogError("Tried to delete playlist {playlistId} but user {userId} doesn't exist", 
                request.PlaylistId, request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }
        
        var playlist = user.RemovePlaylist(request.PlaylistId);
        
        if (playlist == null)
        {
            _logger.LogError(
                "Tried to delete playlist {playlistId} but user {userId} does not have this playlist",
                request.PlaylistId, request.UserId);
            return Result.Failure(UserPlaylistErrors.Ownership);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}