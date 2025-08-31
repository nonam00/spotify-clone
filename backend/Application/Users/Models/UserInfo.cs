namespace Application.Users.Models;

public class UserInfo
{
    public string Email { get; init; } = null!;
    public string? FullName { get; init; }
    public string? AvatarPath { get; init; }
}