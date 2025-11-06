using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.DeletePlaylist;

public class DeletePlaylistCommandHandler : ICommandHandler<DeletePlaylistCommand>
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

    public async Task Handle(DeletePlaylistCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithPlaylists(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogError("Tried to delete playlist {playlistId} but user {userId} doesn't exist", 
                request.PlaylistId, request.UserId);
            throw new ArgumentException("User doesn't exist");
        }
        
        var playlist = user.RemovePlaylist(request.PlaylistId);
        
        if (playlist == null)
        {
            _logger.LogError(
                "Tried to delete playlist {playlistId} but user {userId} does not have this playlist",
                request.PlaylistId, request.UserId);
            throw new ArgumentException("Playlist does not exist or does not belong to you");
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}