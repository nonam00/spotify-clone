using FluentValidation;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQueryValidator : AbstractValidator<GetPlaylistByIdQuery>
{
    public GetPlaylistByIdQueryValidator()
    {
        RuleFor(q => q.Id).NotEqual(Guid.Empty);
    }
}