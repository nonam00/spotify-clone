using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Commands.UnlikeSong;

public class UnlikeSongCommandHandler : ICommandHandler<UnlikeSongCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnlikeSongCommandHandler> _logger;
    public UnlikeSongCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<UnlikeSongCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UnlikeSongCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithSongs(request.UserId, cancellationToken);
        
        if (user == null)
        {
            _logger.LogError(
                "Tried to unlike song {songId} but user {userId} doesn't exist", 
                request.SongId, request.UserId);
            throw new ArgumentException("User doesn't exist");
        }
        
        if (!user.UnlikeSong(request.SongId))
        {
            _logger.LogError(
                "User with id {userId} tried to like song {songId} but he has not liked this song",
                request.UserId, request.SongId);
            throw new ArgumentException("You have not liked this song");
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}