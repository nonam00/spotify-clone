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

    public Result<Moderator> CreateModerator(
        Email email, PasswordHash passwordHash, string? fullName = null, bool isSuper = false)
    {
        if (!IsActive)
        {
            return Result<Moderator>.Failure(ModeratorDomainErrors.NotActive);
        }
        
        if (!Permissions.CanManageModerators)
        {
            return Result<Moderator>.Failure(ModeratorDomainErrors.CannotManageModerators);
        }

        var permissions = isSuper
            ? ModeratorPermissions.CreateSuperAdmin()
            : ModeratorPermissions.CreateDefault();
        
        var newModerator = Create(email, passwordHash, fullName, permissions);

        return Result<Moderator>.Success(newModerator);
    }

    public Result UpdateModeratorPermissions(Moderator moderatorToUpdate, ModeratorPermissions permissions)
    {
        if (moderatorToUpdate.Id == Id)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageHimself);
        }
        
        if (!IsActive)
        {
            return Result<Moderator>.Failure(ModeratorDomainErrors.NotActive);
        }
        
        if (!Permissions.CanManageModerators)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }
        moderatorToUpdate.Permissions =  permissions;
        return Result.Success();
    }

    public Result ActivateModerator(Moderator moderatorToActivate)
    {
        if (moderatorToActivate.Id == Id)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageHimself);
        }
        
        if (!IsActive)
        {
            return Result<Moderator>.Failure(ModeratorDomainErrors.NotActive);
        }
        
        if (!Permissions.CanManageModerators)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }
        var activationResult = moderatorToActivate.Activate();
        return activationResult;
    }

    public Result DeactivateModerator(Moderator moderatorToDeactivate)
    {
        if (moderatorToDeactivate.Id == Id)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageHimself);
        }
        
        if (!IsActive)
        {
            return Result<Moderator>.Failure(ModeratorDomainErrors.NotActive);
        }
        
        if (!Permissions.CanManageModerators)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }
        
        var activationResult = moderatorToDeactivate.Deactivate();
        return activationResult;
    }
    
    internal Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.AlreadyDeactivated);
        }
        IsActive = false;
        return Result.Success();
    }

    internal Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.AlreadyActive);
        }
        IsActive = true;
        return Result.Success();
    }
}

public static class ModeratorDomainErrors
{
    public static readonly Error NotActive =
        new(nameof(NotActive), "The moderator has not been activated and cannot perform actions");
    
    public static readonly Error AlreadyActive =
        new(nameof(AlreadyActive), "Moderator is already active");
    
    public static readonly Error AlreadyDeactivated =
        new(nameof(AlreadyDeactivated), "Moderator is already deactivated");
    
    public static readonly Error CannotManageModerators =
        new(nameof(CannotManageModerators), "Moderator cannot manage moderators");
    
    public static readonly Error CannotManageHimself =
        new(nameof(CannotManageHimself), "Moderator cannot manage himself");
}