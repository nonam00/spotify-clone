using Application.Moderators.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Moderators.Queries.GetModeratorList;

public record GetModeratorListQuery : IQuery<Result<ModeratorListVm>>;

