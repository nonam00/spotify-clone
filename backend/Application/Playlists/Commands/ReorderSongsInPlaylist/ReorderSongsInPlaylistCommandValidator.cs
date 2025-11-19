using FluentValidation;

namespace Application.Playlists.Commands.ReorderSongsInPlaylist;

public class ReorderSongsInPlaylistCommandValidator : AbstractValidator<ReorderSongsInPlaylistCommand>
{
    public ReorderSongsInPlaylistCommandValidator()
    {
        RuleFor(c => c.PlaylistId)
            .NotEqual(Guid.Empty)
            .WithMessage("Playlist ID is required")
            .WithErrorCode("400");
        
        RuleFor(c => c.SongIds)
            .NotEmpty()
            .WithMessage("Song list is required")
            .WithErrorCode("400")
            .ForEach(i => i
                .NotEqual(Guid.Empty)
                .WithMessage("Song ID is required")
                .WithErrorCode("400"));
    }
}