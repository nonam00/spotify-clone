using Domain.Common;

namespace Application.Users.Errors;

public static class RefreshTokenErrors
{
    public static readonly Error RelevantNotFound =
        new(nameof(RelevantNotFound), "Refresh token does not exist or expired");
}