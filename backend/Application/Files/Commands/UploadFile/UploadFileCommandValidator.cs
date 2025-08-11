using FluentValidation;

namespace Application.Files.Commands.UploadFile;

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(c => c.FileStream).NotNull();
    }
}