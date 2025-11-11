using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.CleanupNonActiveUsers;

public class CleanupNonActiveUsersCommand : ICommand<Result>;