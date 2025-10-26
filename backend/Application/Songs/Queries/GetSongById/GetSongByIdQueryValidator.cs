using FluentValidation;

namespace Application.Songs.Queries.GetSongById;

public class GetSongByIdQueryValidator : AbstractValidator<GetSongByIdQuery>
{
    public GetSongByIdQueryValidator()
    {
        RuleFor(command => command.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");    
    }
}