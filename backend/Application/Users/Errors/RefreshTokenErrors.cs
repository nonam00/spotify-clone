using Domain.Common;

namespace Application.Users.Errors;

public static class RefreshTokenErrors
{
    public static readonly Error NotFound = new(
        nameof(NotFound),
        "Refresh token does not exist");
    
    public static readonly Error RelevantNotFound = new(
        nameof(RelevantNotFound),
        "Refresh token does not exist or expired");
}