using Application.Moderators.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Moderators.Queries.GetModeratorInfo;

public record GetModeratorInfoQuery(Guid ModeratorId) : IQuery<Result<ModeratorInfo>>;