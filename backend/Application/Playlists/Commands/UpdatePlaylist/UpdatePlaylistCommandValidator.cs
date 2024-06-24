using FluentValidation;

namespace Application.Playlists.Commands.UpdatePlaylist
{
    public class UpdatePlaylistCommandValidator
        : AbstractValidator<UpdatePlaylistCommand>
    {
        public UpdatePlaylistCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(Guid.Empty);
        }
    }
}
