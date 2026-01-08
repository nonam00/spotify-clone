using FluentValidation;

namespace Application.Songs.Commands.DeleteSongs;

public class DeleteSongsCommandValidator : AbstractValidator<DeleteSongsCommand>
{
    public DeleteSongsCommandValidator()
    {
        RuleFor(c => c.SongIds)
            .NotEmpty()
                .WithMessage("Song ids list cannot be empty")
            .ForEach(c => c
                .NotEqual(Guid.Empty)
                .WithMessage("Song ID is required")
                .WithErrorCode("400"));
    }
}