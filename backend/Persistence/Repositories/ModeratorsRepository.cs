using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Moderators.Interfaces;
using Application.Moderators.Models;

namespace Persistence.Repositories;

public class ModeratorsRepository : IModeratorsRepository
{
    private readonly AppDbContext _dbContext;

    public ModeratorsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask Add(Moderator moderator, CancellationToken cancellationToken = default)
    {
        await _dbContext.Moderators.AddAsync(moderator, cancellationToken).ConfigureAwait(false);
    }

    public Task<Moderator?> GetById(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Moderators.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<Moderator?> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Moderators
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Email == email, cancellationToken);
    }
    
    public Task<List<ModeratorVm>> GetList(CancellationToken cancellationToken = default)
    {
        return _dbContext.Moderators
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }
    
    public void Update(Moderator moderator) => _dbContext.Moderators.Update(moderator);

    private static readonly Expression<Func<Moderator, ModeratorVm >> ToVmExpression = moderator =>
        new ModeratorVm(
            moderator.Id,
            moderator.Email,
            moderator.FullName ?? "",
            moderator.IsActive,
            moderator.CreatedAt,
            new ModeratorPermissionsVm(
                moderator.Permissions.CanManageUsers,
                moderator.Permissions.CanManageContent,
                moderator.Permissions.CanViewReports,
                moderator.Permissions.CanManageModerators));
}