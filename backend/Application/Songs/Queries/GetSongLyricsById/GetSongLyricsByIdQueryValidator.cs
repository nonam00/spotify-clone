using FluentValidation;

namespace Application.Songs.Queries.GetSongLyricsById;

public class GetSongLyricsByIdQueryValidator : AbstractValidator<GetSongLyricsByIdQuery>
{
    public GetSongLyricsByIdQueryValidator()
    {
        RuleFor(command => command.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");    
    }
}