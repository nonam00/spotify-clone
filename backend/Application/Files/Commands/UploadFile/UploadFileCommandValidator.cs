using FluentValidation;

namespace Application.Files.Commands.UploadFile
{
    public class UploadFileCommandValidator
        : AbstractValidator<UploadFileCommand>
    {
        public UploadFileCommandValidator()
        {
            RuleFor(c => c.FileStream).NotNull();
            RuleFor(c => c.ContentType)
              .Must(type => type.Equals("audio") ||
                            type.Equals("image"))
              .WithMessage("Wrong file content type");
        }
    }
}
