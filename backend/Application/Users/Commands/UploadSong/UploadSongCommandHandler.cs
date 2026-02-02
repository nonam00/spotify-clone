using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.UploadSong;

public class UploadSongCommandHandler : ICommandHandler<UploadSongCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadSongCommandHandler> _logger;

    public UploadSongCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<UploadSongCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UploadSongCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithUploadedSongs(request.UserId, cancellationToken);
        
        if (user is null)
        {
            _logger.LogError("Tried to upload song but user {UserId} doesnt exist", request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }
        
        var audioPath = new FilePath(request.SongPath);
        var imagePath = new FilePath(request.ImagePath);
        
        var uploadSongResult = user.UploadSong(request.Title.Trim(), request.Author.Trim(), audioPath, imagePath);
        if (uploadSongResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to upload song but domain error occurred: {DomainErrorDescription}",
                request.UserId, uploadSongResult.Error);
            return uploadSongResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "User {UserId} successfully uploaded song {SongId}",
            request.UserId, uploadSongResult.Value.Id);
        
        return Result.Success();
    }
}