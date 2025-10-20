using FluentValidation;

namespace Application.Songs.Commands.CreateSong;

public class CreateSongCommandValidator : AbstractValidator<CreateSongCommand>
{
    public CreateSongCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");

        RuleFor(command => command.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .WithErrorCode("400")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters")
            .WithErrorCode("400");

        RuleFor(command => command.Author)
            .NotEmpty()
            .WithMessage("Author is required")
            .WithErrorCode("400")
            .MaximumLength(200)
            .WithMessage("Author cannot exceed 200 characters")
            .WithErrorCode("400");

        RuleFor(command => command.SongPath)
            .NotEmpty()
            .WithMessage("Song path is required")
            .WithErrorCode("400")
            .MaximumLength(500)
            .WithMessage("Song path cannot exceed 500 characters")
            .WithErrorCode("400");

        RuleFor(command => command.ImagePath)
            .NotEmpty()
            .WithMessage("Image path is required")
            .WithErrorCode("400")
            .MaximumLength(500)
            .WithMessage("Image path cannot exceed 500 characters")
            .WithErrorCode("400");
    }
}