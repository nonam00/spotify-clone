using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Common;

namespace Application.Users.Commands.UploadSong;

public class UploadSongCommandHandler : ICommandHandler<UploadSongCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadSongCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UploadSongCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithUploadedSongs(request.UserId, cancellationToken);
        
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        
        var audioPath = new FilePath(request.SongPath);
        var imagePath = new FilePath(request.ImagePath);
        
        user.UploadSong(request.Title.Trim(), request.Author.Trim(), audioPath, imagePath);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}