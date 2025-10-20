using Application.Shared.Messaging;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : ICommand<string?>
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? AvatarPath { get; set; }
}