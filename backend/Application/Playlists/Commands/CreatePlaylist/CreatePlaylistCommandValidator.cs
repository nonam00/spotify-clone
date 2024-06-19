using FluentValidation;

namespace Application.Playlists.Commands.CreatePlaylist
{
    public class CreatePlaylistCommandValidator
      : AbstractValidator<CreatePlaylistCommand>
    {
        public CreatePlaylistCommandValidator()
        {
            RuleFor(c => c.UserId).NotEqual(Guid.Empty);
        }
    }
}
