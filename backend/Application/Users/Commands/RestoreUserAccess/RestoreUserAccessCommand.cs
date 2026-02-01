using Application.Shared.Messaging;
using Application.Shared.Models;
using Domain.Common;

namespace Application.Users.Commands.RestoreUserAccess;

public record RestoreUserAccessCommand(string Email, string RestoreCode) : ICommand<Result<TokenPair>>;