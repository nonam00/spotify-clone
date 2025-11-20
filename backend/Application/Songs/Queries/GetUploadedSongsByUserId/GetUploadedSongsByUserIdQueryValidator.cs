using FluentValidation;

namespace Application.Songs.Queries.GetUploadedSongsByUserId;

public class GetUploadedSongsByUserIdQueryValidator : AbstractValidator<GetUploadedSongsByUserIdQuery>
{
    public GetUploadedSongsByUserIdQueryValidator()
    {
        RuleFor(q => q.UserId).NotEmpty();
    }
}

