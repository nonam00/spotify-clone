using FluentValidation;

namespace Application.Songs.Commands.PublishSongs;

public class PublishSongsCommandValidator : AbstractValidator<PublishSongsCommand>
{
    public PublishSongsCommandValidator()
    {
        RuleFor(c => c.SongIds)
            .ForEach(c => c
                .NotEqual(Guid.Empty)
                .WithMessage("Song ID is required")
                .WithErrorCode("400"));
    }
}