using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Models;

namespace Application.Users.Commands.CreatePlaylist;

public class CreatePlaylistCommandHandler : ICommandHandler<CreatePlaylistCommand, Result<Guid>>
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

    public async Task<Result<Guid>> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithPlaylists(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogError("Tried to create a playlist but that user {userId} doesn't exist", request.UserId);
            return Result<Guid>.Failure(UserErrors.NotFound);
        }
        
        var playlist = user.CreatePlaylist();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<Guid>.Success(playlist.Id);
    }
}