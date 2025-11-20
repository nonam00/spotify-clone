using Application.Moderators.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Queries.GetModeratorInfo;

public record GetModeratorInfoQuery(Guid ModeratorId) : IQuery<Result<ModeratorInfo>>;