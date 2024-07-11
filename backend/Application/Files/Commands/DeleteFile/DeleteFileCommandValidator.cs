using FluentValidation;

namespace Application.Files.Commands.DeleteFile
{
    public class DeleteFileCommandValidator
        : AbstractValidator<DeleteFileCommand>
    {
        public DeleteFileCommandValidator()
        {
            RuleFor(c => c.FileName).NotEqual(string.Empty);
        }
    }
}

