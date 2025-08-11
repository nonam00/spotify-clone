using FluentValidation;

namespace Application.Files.Queries.GetFile;

public class GetFileQueryValidator : AbstractValidator<GetFileQuery>
{
    public GetFileQueryValidator()
    {
        RuleFor(q => q.Name).NotEqual(string.Empty);
    }
}