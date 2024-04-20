using FluentValidation;

namespace Application.LikedSongs.Commands.CreateLikedSong
{
    public class CreateLikedSongCommandValidator
        : AbstractValidator<CreateLikedSongCommand>
    {
        public CreateLikedSongCommandValidator()
        {
            RuleFor(command => command.UserId).NotEqual(Guid.Empty);
            RuleFor(command => command.SongId).NotEqual(Guid.Empty);
        }
    }
}
