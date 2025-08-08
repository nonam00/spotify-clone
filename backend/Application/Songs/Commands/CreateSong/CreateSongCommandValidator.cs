using FluentValidation;

namespace Application.Songs.Commands.CreateSong;

public class CreateSongCommandValidator : AbstractValidator<CreateSongCommand>
{
    public CreateSongCommandValidator()
    {
        RuleFor(create => create.UserId).NotEqual(Guid.Empty);
        RuleFor(create => create.Title.Trim()).NotEmpty();
        RuleFor(create => create.Author.Trim()).NotEmpty();
        RuleFor(create => create.SongPath.Trim()).NotEmpty();
        RuleFor(create => create.ImagePath.Trim()).NotEmpty();
    }
}