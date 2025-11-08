using Application.Shared.Data;

namespace Application.Users.Errors;

public static class UserLikeErrors
{
    public static readonly Error AlreadyLiked = new(
        nameof(AlreadyLiked),
        "You already liked this song.");
    
    public static readonly Error NotLiked = new(
        nameof(AlreadyLiked),
        "You did not liked this song.");
}