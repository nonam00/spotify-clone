using Application.Shared.Data;

namespace Application.Moderators.Errors;

public static class ModeratorErrors
{
    public static readonly Error NotFound = new(
        nameof(NotFound),
        "Moderator not found");
    
    public static readonly Error InvalidCredentials = new(
        nameof(InvalidCredentials),
        "Invalid credentials");
    
    public static readonly Error AlreadyExistButNotActive = new(
        nameof(AlreadyExistButNotActive),
        "Ask your admin to activate your account.");
    
    public static readonly Error AlreadyExist = new(
        nameof(AlreadyExist),
        "Moderator already exists.");
}
