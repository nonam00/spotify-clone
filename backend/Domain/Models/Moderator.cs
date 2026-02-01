using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Models;

// Moderator Aggregate Root
public class Moderator : AggregateRoot<Guid>
{
    public Email Email { get; private init; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public string? FullName { get; private init; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ModeratorPermissions Permissions { get; private set; } = null!;

    private Moderator() { }

    public static Moderator Create(
        Email email, PasswordHash passwordHash, string? fullName = null, ModeratorPermissions? permissions = null)
    {
        var moderator = new Moderator
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FullName = fullName?.Trim(),
            IsActive = true,
            Permissions = permissions ?? ModeratorPermissions.CreateDefault(),
            CreatedAt = DateTime.UtcNow
        };

        return moderator;
    }

    public void ChangePassword(PasswordHash newPasswordHash) => PasswordHash = newPasswordHash;

    public Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.AlreadyDeactivated);
        }
        IsActive = false;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.AlreadyActive);
        }
        IsActive = true;
        return Result.Success();
    }

    public Result UpdateModeratorPermissions(Moderator moderatorToUpdate, ModeratorPermissions permissions)
    {
        if (!Permissions.CanManageModerators)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }
        moderatorToUpdate.Permissions =  permissions;
        return Result.Success();
    }
}

public static class ModeratorDomainErrors
{
    public static readonly Error AlreadyActive =
        new Error(nameof(AlreadyActive), "Moderator is already active");
    
    public static readonly Error AlreadyDeactivated =
        new Error(nameof(AlreadyDeactivated), "Moderator is already deactivated");
    
    public static readonly Error CannotManageModerators =
        new Error(nameof(CannotManageModerators), "Moderator cannot manage moderators");
}
