using FluentValidation;

namespace Application.Moderators.Commands.PublishSongs;

public class PublishSongsCommandValidator : AbstractValidator<PublishSongsCommand>
{
    public PublishSongsCommandValidator()
    {
        RuleFor(c => c.ModeratorId)
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