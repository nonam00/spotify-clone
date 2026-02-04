using Domain.Common;

namespace Domain.Errors;

public static class ModeratorDomainErrors
{
    public static readonly Error NotActive =
        new(nameof(NotActive), "The moderator has not been activated and cannot perform actions.");
    
    public static readonly Error AlreadyActive =
        new(nameof(AlreadyActive), "Moderator is already active.");
    
    public static readonly Error AlreadyDeactivated =
        new(nameof(AlreadyDeactivated), "Moderator is already deactivated.");
    
    public static readonly Error CannotManageModerators =
        new(nameof(CannotManageModerators), "Moderator cannot manage moderators.");
    
    public static readonly Error CannotManageHimself =
        new(nameof(CannotManageHimself), "Moderator cannot manage himself.");
    
    public static readonly Error CannotManageContent =
        new(nameof(CannotManageContent), "Moderator cannot manage content.");
    
    public static readonly Error CannotManageEmptySongList =
        new(nameof(CannotManageEmptySongList), "Moderator cannot manage empty song list.");
    
    public static readonly Error CannotManageUsers =
        new(nameof(CannotManageUsers), "Moderator cannot manage users.");
}