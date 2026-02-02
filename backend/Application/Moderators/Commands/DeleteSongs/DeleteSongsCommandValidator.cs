using FluentValidation;

namespace Application.Moderators.Commands.DeleteSongs;

public class DeleteSongsCommandValidator : AbstractValidator<DeleteSongsCommand>
{
    public DeleteSongsCommandValidator()
    {
        RuleFor(x => x.ModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Moderator ID is required")
            .WithErrorCode("400");
        
        RuleFor(c => c.SongIds)
            .NotEmpty()
                .WithMessage("Song ids list cannot be empty")
            .ForEach(c => c
                .NotEqual(Guid.Empty)
                .WithMessage("Song ID is required")
                .WithErrorCode("400"));
    }
}