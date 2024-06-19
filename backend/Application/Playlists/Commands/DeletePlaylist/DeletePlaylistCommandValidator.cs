using FluentValidation;

namespace Application.Playlists.Commands.DeletePlaylist
{
    public class DeletePlaylistCommandValidator
      : AbstractValidator<DeletePlaylistCommand>
    {
        public DeletePlaylistCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(Guid.Empty);
        }
    }
}
