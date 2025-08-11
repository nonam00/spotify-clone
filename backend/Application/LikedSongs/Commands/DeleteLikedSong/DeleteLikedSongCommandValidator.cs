using FluentValidation;

namespace Application.LikedSongs.Commands.DeleteLikedSong;

public class DeleteLikedSongCommandValidator : AbstractValidator<DeleteLikedSongCommand>
{
    public DeleteLikedSongCommandValidator()
    {
        RuleFor(command => command.UserId).NotEqual(Guid.Empty);
        RuleFor(command => command.SongId).NotEqual(Guid.Empty);
    }
}