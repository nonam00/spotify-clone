using FluentValidation;

namespace Application.Playlists.Commands.DeletePlaylist;

public class DeletePlaylistCommandValidator : AbstractValidator<DeletePlaylistCommand>
{
    public DeletePlaylistCommandValidator()
    {
        RuleFor(c => c.UserId).NotEqual(Guid.Empty);
        RuleFor(c => c.PlaylistId).NotEqual(Guid.Empty);
    }
}