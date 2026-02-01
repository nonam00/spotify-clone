using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.CleanupNonActiveUsers;

public class CleanupNonActiveUsersCommand : ICommand<Result>;