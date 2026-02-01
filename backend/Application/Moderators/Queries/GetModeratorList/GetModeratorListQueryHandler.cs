using Application.Moderators.Interfaces;
using Application.Moderators.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Moderators.Queries.GetModeratorList;

public class GetModeratorListQueryHandler : IQueryHandler<GetModeratorListQuery, Result<ModeratorListVm>>
{
    private readonly IModeratorsRepository _moderatorsRepository;

    public GetModeratorListQueryHandler(IModeratorsRepository moderatorsRepository) 
    {
        _moderatorsRepository = moderatorsRepository;
    }

    public async Task<Result<ModeratorListVm>> Handle(GetModeratorListQuery request, CancellationToken cancellationToken)
    {
        var moderators = await _moderatorsRepository.GetList(cancellationToken).ConfigureAwait(false);
        return Result<ModeratorListVm>.Success(new ModeratorListVm(moderators));
    }
}