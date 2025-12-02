using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Commands.RestoreUserAccess;

public record RestoreUserAccessCommand(string Email, string RestoreCode) : ICommand<Result<TokenPair>>;