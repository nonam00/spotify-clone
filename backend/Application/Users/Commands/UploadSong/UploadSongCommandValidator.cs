using FluentValidation;

namespace Application.Users.Commands.UploadSong;

public class UploadSongCommandValidator : AbstractValidator<UploadSongCommand>
{
    public UploadSongCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");

        RuleFor(command => command.Title.Trim())
            .NotEmpty()
            .WithMessage("Title is required")
            .WithErrorCode("400")
            .MaximumLength(255)
            .WithMessage("Title cannot exceed 255 characters")
            .WithErrorCode("400");

        RuleFor(command => command.Author.Trim())
            .NotEmpty()
            .WithMessage("Author is required")
            .WithErrorCode("400")
            .MaximumLength(255)
            .WithMessage("Author cannot exceed 200 characters")
            .WithErrorCode("400");

        RuleFor(command => command.SongPath.Trim())
            .Must(image => !string.IsNullOrWhiteSpace(image))
            .WithMessage("Song path is required")
            .WithErrorCode("400")
            .MaximumLength(255)
            .WithMessage("Song path cannot exceed 255 characters")
            .WithErrorCode("400");

        RuleFor(command => command.ImagePath.Trim())
            .Must(image => !string.IsNullOrWhiteSpace(image))
            .WithMessage("Image path is required")
            .WithErrorCode("400")
            .MaximumLength(255)
            .WithMessage("Image path cannot exceed 255 characters")
            .WithErrorCode("400");
    }
}