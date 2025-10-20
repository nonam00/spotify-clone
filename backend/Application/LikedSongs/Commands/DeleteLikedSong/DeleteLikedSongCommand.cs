using Application.Shared.Messaging;

namespace Application.LikedSongs.Commands.DeleteLikedSong;

public class DeleteLikedSongCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid SongId { get; init; }
}