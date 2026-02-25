using Domain.Models;
using Application.Moderators.Models;

namespace Application.Moderators.Interfaces;

public interface IModeratorsRepository
{
    ValueTask Add(Moderator moderator, CancellationToken cancellationToken = default);
    Task<Moderator?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<Moderator?> GetByEmail(string email, CancellationToken cancellationToken = default);
    Task<List<ModeratorVm>> GetList(CancellationToken cancellationToken = default);
    void Update(Moderator moderator);
}