using FluentValidation;

namespace Application.Files.Commands.DeleteFile;

public class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileCommandValidator()
    {
        RuleFor(c => c.Name).NotEqual(string.Empty);
    }
}