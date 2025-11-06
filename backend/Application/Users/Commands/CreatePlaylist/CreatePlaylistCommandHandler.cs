using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CreatePlaylist;

public class CreatePlaylistCommandHandler : ICommandHandler<CreatePlaylistCommand, Guid>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePlaylistCommandHandler> _logger;
    
    public CreatePlaylistCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork, ILogger<CreatePlaylistCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithPlaylists(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogError("Tried to create a playlist but that user {userId} doesn't exist", request.UserId);
            throw new ArgumentException("User doesn't exist");
        }
        
        var playlist = user.CreatePlaylist();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return playlist.Id;
    }
}