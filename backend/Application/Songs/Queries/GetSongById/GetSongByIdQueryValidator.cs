using FluentValidation;

namespace Application.Songs.Queries.GetSongById;

public class GetSongByIdQueryValidator : AbstractValidator<GetSongByIdQuery>
{
    public GetSongByIdQueryValidator()
    {
        RuleFor(q => q.SongId).NotEqual(Guid.Empty);
    }
}