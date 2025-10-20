using Domain;
using Application.LikedSongs.Interfaces;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.LikedSongs.Commands.CreateLikedSong;

public class CreateLikedSongCommandHandler : ICommandHandler<CreateLikedSongCommand, string>
{
    private readonly ILikedSongsRepository _likedSongsRepository;
    private readonly IUsersRepository _usersRepository;
    public CreateLikedSongCommandHandler(ILikedSongsRepository likedSongsRepository, IUsersRepository usersRepository)
    {
        _likedSongsRepository = likedSongsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<string> Handle(CreateLikedSongCommand request, CancellationToken cancellationToken)
    {
        var isUserActivated = await _usersRepository.CheckIfActivated(request.UserId, cancellationToken);
        
        if (!isUserActivated)
        {
            throw new Exception("Account is not activated. User can not perform any actions");
        }
        
        var likedSong = new LikedSong
        {
            UserId = request.UserId,
            SongId = request.SongId
        };

        await _likedSongsRepository.Add(likedSong, cancellationToken);

        return $"{likedSong.UserId}:{likedSong.SongId}";
    }
}