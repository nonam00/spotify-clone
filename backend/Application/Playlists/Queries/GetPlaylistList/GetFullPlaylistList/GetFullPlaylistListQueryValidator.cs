using FluentValidation;

namespace Application.Playlists.Queries.GetPlaylistList.GetFullPlaylistList;

public class GetFullPlaylistListQueryValidator : AbstractValidator<GetFullPlaylistListQuery>
{
    public GetFullPlaylistListQueryValidator()
    {
        RuleFor(q => q.UserId).NotEqual(Guid.Empty);
    }
}