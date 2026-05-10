using FluentValidation;

namespace Application.Songs.Commands.UpdateTranscribeInformation;

public class UpdateTranscribeInformationCommandValidator : AbstractValidator<UpdateTranscribeInformationCommand>
{
    public UpdateTranscribeInformationCommandValidator()
    {
        RuleFor(command => command.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");
    }
}