using FluentValidation;

namespace Application.Songs.Commands.UpdateTranscribeInformation;

public class UpdateTranscribeInformationCommandValidator : AbstractValidator<UpdateTranscribeInformationCommand>
{
    public UpdateTranscribeInformationCommandValidator()
    {
        RuleFor(x => x.SongId).NotEqual(Guid.Empty);
    }
}