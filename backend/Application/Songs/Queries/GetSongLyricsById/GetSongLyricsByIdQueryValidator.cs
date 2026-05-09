using FluentValidation;

namespace Application.Songs.Queries.GetSongLyricsById;

public class GetSongLyricsByIdCommandValidator : AbstractValidator<GetSongLyricsByIdQuery>
{
    public GetSongLyricsByIdCommandValidator()
    {
        RuleFor(command => command.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");    
    }
}