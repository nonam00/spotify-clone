using FluentValidation;

namespace Application.Playlists.Commands.AddSongToPlaylist;

public class AddSongToPlaylistCommandValidator : AbstractValidator<AddSongToPlaylistCommand>
{
    public AddSongToPlaylistCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
        
        RuleFor(c => c.PlaylistId)
            .NotEqual(Guid.Empty)
            .WithMessage("Playlist ID is required")
            .WithErrorCode("400");
        
        RuleFor(c => c.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");
    }
}