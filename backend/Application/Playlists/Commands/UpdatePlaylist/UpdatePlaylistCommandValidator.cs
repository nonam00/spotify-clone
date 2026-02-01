using FluentValidation;

namespace Application.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommandValidator : AbstractValidator<UpdatePlaylistCommand>
{
    public UpdatePlaylistCommandValidator()
    {
        RuleFor(c => c.PlaylistId)
            .NotEqual(Guid.Empty)
            .WithMessage("Playlist ID is required")
            .WithErrorCode("400");
        
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");

        RuleFor(command => command.Title.Trim())
            .Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage("Title must be not empty or white space")
            .WithErrorCode("400");;
    }
}