using Domain.Common;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Commands.RestoreUserAccess;

public record RestoreUserAccessCommand(string Email, string RestoreCode) : ICommand<Result<TokenPair>>;