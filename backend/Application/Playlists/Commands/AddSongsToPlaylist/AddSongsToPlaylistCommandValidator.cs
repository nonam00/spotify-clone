using FluentValidation;

namespace Application.Playlists.Commands.AddSongsToPlaylist;

public class AddSongsToPlaylistCommandValidator : AbstractValidator<AddSongsToPlaylistCommand>
{
    public AddSongsToPlaylistCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
        
        RuleFor(c => c.PlaylistId)
            .NotEqual(Guid.Empty)
            .WithMessage("Playlist ID is required")
            .WithErrorCode("400");

        RuleFor(c => c.SongIds)
            .ForEach(c => c
                .NotEqual(Guid.Empty)
                .WithMessage("Song ID is required")
                .WithErrorCode("400"));
    }
}