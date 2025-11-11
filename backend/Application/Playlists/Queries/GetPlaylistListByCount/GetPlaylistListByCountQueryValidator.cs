using FluentValidation;

namespace Application.Playlists.Queries.GetPlaylistListByCount;

public class GetPlaylistListByCountQueryValidator : AbstractValidator<GetPlaylistListByCountQuery>
{
    public GetPlaylistListByCountQueryValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
        
        RuleFor(q => q.Count)
            .GreaterThan(0)
            .WithMessage("Count must be greater than 0")
            .WithErrorCode("400");
    }
}