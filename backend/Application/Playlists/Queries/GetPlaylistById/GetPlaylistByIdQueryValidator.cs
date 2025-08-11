using FluentValidation;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQueryValidator : AbstractValidator<GetPlaylistByIdQuery>
{
    public GetPlaylistByIdQueryValidator()
    {
        RuleFor(q => q.PlaylistId).NotEqual(Guid.Empty);
        RuleFor(q => q.UserId).NotEqual(Guid.Empty);
    }
}