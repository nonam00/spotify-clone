using Domain.Models;
using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;

namespace Application.Songs.Commands.CreateSong;

public class CreateSongCommandHandler : ICommandHandler<CreateSongCommand, Guid>
{
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSongCommandHandler(ISongsRepository songsRepository, IUnitOfWork unitOfWork)
    {
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSongCommand request, CancellationToken cancellationToken)
    {
        var audioPath = new FilePath(request.SongPath);
        var imagePath = new FilePath(request.ImagePath);
        var song = Song.Create(request.Title, audioPath, imagePath, request.Author, request.UserId);
        
        await _songsRepository.Add(song, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return song.Id;
    }
}