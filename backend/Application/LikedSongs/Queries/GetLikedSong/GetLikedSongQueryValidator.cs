using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSong
{
    public class GetLikedSongQueryValidator : AbstractValidator<GetLikedSongQuery>
    {
        public GetLikedSongQueryValidator()
        {
            RuleFor(q => q.UserId).NotEqual(Guid.Empty);
            RuleFor(q => q.SongId).NotEqual(Guid.Empty);
        }
    }
}
