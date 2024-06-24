using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetFullLikedSongList
{
    public class GetFullLikedSongListQueryValidator
        : AbstractValidator<GetFullLikedSongListQuery>
    {
        public GetFullLikedSongListQueryValidator()
        {
            RuleFor(query => query.UserId).NotEqual(Guid.Empty);
        }
    }
}
