using FluentValidation;

namespace Application.LikedSongs.Queries.CheckLikedSong
{
    public class CheckLikedSongQueryValidator : AbstractValidator<CheckLikedSongQuery>
    {
        public CheckLikedSongQueryValidator()
        {
            RuleFor(q => q.UserId).NotEqual(Guid.Empty);
            RuleFor(q => q.SongId).NotEqual(Guid.Empty);
        }
    }
}
