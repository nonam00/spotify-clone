using Microsoft.Extensions.Logging;

using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Moderators.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Moderators.Queries.GetModeratorInfo;

public class GetModeratorInfoQueryHandler : IQueryHandler<GetModeratorInfoQuery, Result<ModeratorInfo>>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly ILogger<GetModeratorInfoQueryHandler> _logger;
    
    public GetModeratorInfoQueryHandler(
        IModeratorsRepository moderatorsRepository,
        ILogger<GetModeratorInfoQueryHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _logger = logger;
    }

    public async Task<Result<ModeratorInfo>> Handle(GetModeratorInfoQuery request, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorsRepository.GetById(request.ModeratorId, cancellationToken);

        if (moderator != null)
        {
            var permissionsVm = new ModeratorPermissionsVm(
                moderator.Permissions.CanManageUsers,
                moderator.Permissions.CanManageContent,
                moderator.Permissions.CanViewReports,
                moderator.Permissions.CanManageModerators);
            
            return Result<ModeratorInfo>.Success(new ModeratorInfo(
                moderator.Id,
                moderator.Email,
                moderator.FullName ?? "",
                moderator.IsActive,
                permissionsVm));
        }
        
        _logger.LogWarning("Tried to get info for non-existing user {userId}", request.ModeratorId);
        return Result<ModeratorInfo>.Failure(ModeratorErrors.NotFound);
    }
}