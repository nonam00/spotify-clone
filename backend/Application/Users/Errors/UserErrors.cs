using Domain.Common;

namespace Application.Users.Errors;

public static class UserErrors
{
    public static readonly Error NotFound = 
        new(nameof(NotFound), "User does not exist.");
    
    public static readonly Error NotFoundWithEmail =
        new(nameof(NotFoundWithEmail), "User with this email does not exist.");
    
    public static readonly Error AlreadyExist =
        new(nameof(AlreadyExist),"User already exists.");

    public static readonly Error AlreadyExistButNotActive = 
        new(nameof(AlreadyExistButNotActive), "Activate your account.");

    public static readonly Error InvalidCredentials = 
        new(nameof(InvalidCredentials), "Invalid email or password.");
    
    public static readonly Error PasswordsMissMatch =
        new(nameof(PasswordsMissMatch), "Passwords do not match."); 
    
    public static readonly Error InvalidVerificationCode =
        new(nameof(InvalidVerificationCode), "Invalid verification code.");
}