using System.Reflection;
using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Tests.Helpers;

public static class ModeratorHelpers
{
    public static Moderator CreateModerator(
        bool isActive = true,
        bool canViewReports = true,
        bool canManageContent = true,
        bool canManageUsers = true)
    {
        var email = new Email($"moderator{Guid.NewGuid().ToString()[..8]}@test.com");
        var passwordHash = new PasswordHash("TestPassword123!");
            
        var permissions = new ModeratorPermissions(
            canManageModerators: false,
            canManageContent: canManageContent,
            canManageUsers: canManageUsers,
            canViewReports: canViewReports);

        var moderator = Moderator.Create(email, passwordHash, "Test Moderator", permissions);
            
        if (!isActive)
        {
            moderator.GetType()
                .GetMethod("Deactivate", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(moderator, null);
        }
            
        return moderator;
    }

    public static Moderator CreateAdmin(bool isActive = true)
    {
        var email = new Email($"superadmin{Guid.NewGuid().ToString()[..8]}@test.com");
        var passwordHash = new PasswordHash("TestPassword123!");
            
        var moderator = Moderator.Create(
            email, 
            passwordHash, 
            "Super Admin",
            ModeratorPermissions.CreateSuperAdmin());
            
        if (!isActive)
        {
            moderator.GetType()
                .GetMethod("Deactivate", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(moderator, null);
        }
            
        return moderator;
    }
}