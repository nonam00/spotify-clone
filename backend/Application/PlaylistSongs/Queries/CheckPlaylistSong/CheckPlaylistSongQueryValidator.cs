using FluentValidation;

namespace Application.PlaylistSongs.Queries.CheckPlaylistSong
{
    public class CheckPlaylistSongQueryValidator
        : AbstractValidator<CheckPlaylistSongQuery>
    {
        public CheckPlaylistSongQueryValidator()
        {
            RuleFor(q => q.PlaylistId).NotEqual(Guid.Empty);
            RuleFor(q => q.SongId).NotEqual(Guid.Empty);
        }
    }
}
