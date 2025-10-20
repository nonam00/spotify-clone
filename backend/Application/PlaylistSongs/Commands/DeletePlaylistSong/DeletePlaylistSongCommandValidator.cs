using FluentValidation;

namespace Application.PlaylistSongs.Commands.DeletePlaylistSong;

public class DeletePlaylistSongCommandValidator : AbstractValidator<DeletePlaylistSongCommand>
{
    public DeletePlaylistSongCommandValidator()
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
            .WithErrorCode("400");;
    }
}