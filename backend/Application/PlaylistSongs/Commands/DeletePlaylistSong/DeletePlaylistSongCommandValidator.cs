using FluentValidation;

namespace Application.PlaylistSongs.Commands.DeletePlaylistSong
{
    public class DeletePlaylistSongCommandValidator
        : AbstractValidator<DeletePlaylistSongCommand>
    {
        public DeletePlaylistSongCommandValidator()
        {
            RuleFor(c => c.UserId).NotEqual(Guid.Empty);
            RuleFor(c => c.PlaylistId).NotEqual(Guid.Empty);
            RuleFor(c => c.SongId).NotEqual(Guid.Empty);
        }
    }
}
