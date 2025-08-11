using FluentValidation;

namespace Application.PlaylistSongs.Commands.CreatePlaylistSong;

public class CreatePlaylistSongCommandValidator : AbstractValidator<CreatePlaylistSongCommand>
{
    public CreatePlaylistSongCommandValidator()
    {
        RuleFor(c => c.UserId).NotEqual(Guid.Empty);
        RuleFor(c => c.PlaylistId).NotEqual(Guid.Empty);
        RuleFor(c => c.SongId).NotEqual(Guid.Empty);
    }
}