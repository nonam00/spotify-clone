using FluentValidation;

namespace Application.Playlists.Queries.GetPlaylistList.GetFullPlaylistList;

public class GetFullPlaylistListQueryValidator : AbstractValidator<GetFullPlaylistListQuery>
{
    public GetFullPlaylistListQueryValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
    }
}