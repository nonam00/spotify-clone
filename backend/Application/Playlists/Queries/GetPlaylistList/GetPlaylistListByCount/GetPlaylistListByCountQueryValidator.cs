using FluentValidation;

namespace Application.Playlists.Queries.GetPlaylistList.GetPlaylistListByCount
{
    public class GetPlaylistListByCountQueryValidator
        : AbstractValidator<GetPlaylistListByCountQuery>
    {
        public GetPlaylistListByCountQueryValidator()
        {
            RuleFor(q => q.UserId).NotEqual(Guid.Empty);
            RuleFor(q => q.Count).GreaterThan(0);
        }
    }
}
