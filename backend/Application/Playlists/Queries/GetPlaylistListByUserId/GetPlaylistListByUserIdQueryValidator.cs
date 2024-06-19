using FluentValidation;

namespace Application.Playlists.Queries.GetPlaylistListByUserId
{
    public class GetPlaylistListByUserIdQueryValidator
      : AbstractValidator<GetPlaylistListByUserIdQuery>
    {
        public GetPlaylistListByUserIdQueryValidator()
        {
            RuleFor(q => q.UserId).NotEqual(Guid.Empty);
        }
    }
}
