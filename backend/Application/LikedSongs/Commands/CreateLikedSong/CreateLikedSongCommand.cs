using Application.Shared.Messaging;

namespace Application.LikedSongs.Commands.CreateLikedSong;

public class CreateLikedSongCommand : ICommand<string>
{
    public Guid UserId { get; init; }
    public Guid SongId { get; init; }
}