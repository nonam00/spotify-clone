using Application.Moderators.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Queries.GetModeratorList;

public record GetModeratorListQuery : IQuery<Result<ModeratorListVm>>;

