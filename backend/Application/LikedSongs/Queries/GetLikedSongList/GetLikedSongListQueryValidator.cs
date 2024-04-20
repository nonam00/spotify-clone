using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSongList
{
    public class GetLikedSongListQueryValidator
        : AbstractValidator<GetLikedSongListQuery>
    {
        public GetLikedSongListQueryValidator()
        {
            RuleFor(query => query.UserId).NotEqual(Guid.Empty);
        }
    }
}
