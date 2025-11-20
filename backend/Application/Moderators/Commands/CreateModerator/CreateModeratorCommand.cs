using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.CreateModerator;

public record CreateModeratorCommand(string Email, string FullName, string Password, bool IsSuper) : ICommand<Result>;