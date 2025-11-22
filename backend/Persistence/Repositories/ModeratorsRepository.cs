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

    public async Task Add(Moderator moderator, CancellationToken cancellationToken = default)
    {
        await _dbContext.Moderators.AddAsync(moderator, cancellationToken);
    }

    public async Task<Moderator?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var moderator = await _dbContext.Moderators
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        
        return moderator;
    }

    public Task<Moderator?> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        var moderator = _dbContext.Moderators
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Email == email, cancellationToken);
        
        return moderator;
    }
    
    public async Task<List<ModeratorVm>> GetList(CancellationToken cancellationToken = default)
    {
        var moderators = await _dbContext.Moderators
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => ToVm(m))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return moderators;
    }
    public void Update(Moderator moderator) => _dbContext.Moderators.Update(moderator);

    private static ModeratorVm ToVm(Moderator moderator)
    {
        return new ModeratorVm(
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
}