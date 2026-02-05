using Domain.Common;
using Domain.Errors;
using Domain.Events;
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
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }
        
        if (!Permissions.CanManageModerators)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }

        if (!moderatorToUpdate.IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }
        
        moderatorToUpdate.Permissions = permissions;
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

    private Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.AlreadyDeactivated);
        }
        IsActive = false;
        return Result.Success();
    }

    private Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.AlreadyActive);
        }
        IsActive = true;
        return Result.Success();
    }

    public Result PublishSong(Song song)
    {
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!Permissions.CanManageContent)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageContent);
        }
        
        var publishResult = song.Publish();
        return publishResult;
    }

    public Result PublishSongs(List<Song> songs)
    {
        if (!songs.Any())
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageEmptySongList);    
        }
        
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!Permissions.CanManageContent)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageContent);
        }

        var publishResults = songs
            .Select(s => new
            {
                Song = s,
                Result = s.Publish(),
            })
            .ToList();
        
        var failed = publishResults
            .Where(s => s.Result.IsFailure)
            .ToList();
        
        if (failed.Any())
        {
            foreach (var markedForDeletion in publishResults.Except(failed))
            {
                markedForDeletion.Song.Unpublish();
            }

            return failed.First().Result;
        }
        
        return Result.Success();
    }
    
    public Result UnpublishSong(Song song)
    {
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!Permissions.CanManageContent)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageContent);
        }
        
        var unpublishResult = song.Unpublish();
        return unpublishResult;
    }
    
    public Result DeleteSong(Song song)
    {
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!Permissions.CanManageContent)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageContent);
        }
        
        var deleteResult = song.MarkForDeletion();

        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }
        
        AddDomainEvent(new ModeratorDeletedSongEvent(song.Id, song.ImagePath, song.SongPath));
        
        return Result.Success();
    }

    public Result DeleteSongs(List<Song> songs)
    {
        if (!songs.Any())
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageEmptySongList);
        }
        
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!Permissions.CanManageContent)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageContent);
        }

        var markForDeletionResults = songs
            .Select(s => new
            {
                Song = s,
                Result = s.MarkForDeletion(),
            })
            .ToList();
        
        var failed = markForDeletionResults
            .Where(s => s.Result.IsFailure)
            .ToList();
        
        if (failed.Any())
        {
            foreach (var markedForDeletion in markForDeletionResults.Except(failed))
            {
                markedForDeletion.Song.CancelMarkForDeletion();
            }

            return failed.First().Result;
        }
        
        foreach (var song in songs)
        {
            AddDomainEvent(new ModeratorDeletedSongEvent(song.Id, song.ImagePath, song.SongPath));
        }
        
        return Result.Success();
    }

    public Result ActivateUser(User user)
    {
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!Permissions.CanManageUsers)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageUsers);
        }
        
        var activateResult = user.Activate();
        return activateResult;
    }

    public Result DeactivateUser(User user)
    {
        if (!IsActive)
        {
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!Permissions.CanManageUsers)
        {
            return Result.Failure(ModeratorDomainErrors.CannotManageUsers);
        }
        
        var oldAvatarPath = user.AvatarPath;
        
        var deactivateResult = user.Deactivate();
        if (deactivateResult.IsFailure)
        {
            return deactivateResult;
        }

        if (!string.IsNullOrWhiteSpace(oldAvatarPath))
        {
            AddDomainEvent(new ModeratorDeactivatedUserEvent(user.Id, oldAvatarPath));
        }
        
        return Result.Success();
    }
}