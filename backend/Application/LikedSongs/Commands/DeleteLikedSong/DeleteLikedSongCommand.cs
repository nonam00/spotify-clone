using MediatR;

namespace Application.LikedSongs.Commands.DeleteLikedSong;

public class DeleteLikedSongCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid SongId { get; init; }
}