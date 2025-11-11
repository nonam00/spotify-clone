using System.Linq;
using Application.Shared.Data;

namespace Application.Shared.Errors;

public static class ValidationErrors
{
    public static Error CreateFromFluentValidation(FluentValidation.Results.ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
            .ToList();
        
        var description = string.Join("; ", errors);
        
        return new Error(
            "ValidationError",
            description);
    }
}

