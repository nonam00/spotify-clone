using FluentValidation;

namespace Application.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommandValidator : AbstractValidator<UpdatePlaylistCommand>
{
    public UpdatePlaylistCommandValidator()
    {
        RuleFor(c => c.UserId).NotEqual(Guid.Empty);
        RuleFor(c => c.PlaylistId).NotEqual(Guid.Empty);
    }
}