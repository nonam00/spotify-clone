using Domain.Errors;
using Domain.Models;
using Domain.Tests.Helpers;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Tests.Moderators;

public class ModeratorModeratorTests
{
    private readonly Moderator _activeModerator = ModeratorHelpers.CreateAdmin(isActive: true);
    private readonly Moderator _inactiveModerator = ModeratorHelpers.CreateAdmin(isActive: false);
    
    [Fact]
    public void CreateModerator_WhenModeratorIsActiveAndHasPermissions_ShouldCreateNewModerator()
    {
        // Arrange
        var newEmail = new Email("new@test.com");
        var newPasswordHash = new PasswordHash("NewPassword123!");
        const string newFullName = "New Moderator";

        // Act
        var result = _activeModerator.CreateModerator(newEmail, newPasswordHash, newFullName, isSuper: false);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var newModerator = result.Value;
            
        newModerator.Email.Should().Be(newEmail);
        newModerator.PasswordHash.Should().Be(newPasswordHash);
        newModerator.FullName.Should().Be(newFullName);
        newModerator.IsActive.Should().BeTrue();
        newModerator.Permissions.Should().BeEquivalentTo(ModeratorPermissions.CreateDefault());
    }

    [Fact]
    public void CreateModerator_WhenCreatingSuperAdmin_ShouldCreateWithSuperAdminPermissions()
    {
        // Arrange
        var newEmail = new Email("super@test.com");
        var newPasswordHash = new PasswordHash("NewPassword123!");

        // Act
        var result = _activeModerator.CreateModerator(newEmail, newPasswordHash, isSuper: true);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var newModerator = result.Value;
            
        newModerator.Permissions.CanManageModerators.Should().BeTrue();
        newModerator.Permissions.CanManageContent.Should().BeTrue();
        newModerator.Permissions.CanManageUsers.Should().BeTrue();
    }

    [Fact]
    public void CreateModerator_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var newEmail = new Email("super@test.com");
        var newPasswordHash = new PasswordHash("NewPassword123!");

        // Act
        var result = _inactiveModerator.CreateModerator(newEmail, newPasswordHash);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }

    [Fact]
    public void CreateModerator_WhenModeratorCannotManageModerators_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator();
            
        var newEmail = new Email("new@test.com");
        var newPasswordHash = new PasswordHash("NewPassword123!");

        // Act
        var result = moderatorWithoutPermissions.CreateModerator(newEmail, newPasswordHash);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageModerators);
    }

    [Fact]
    public void UpdateModeratorPermissions_WhenAllConditionsMet_ShouldUpdateSuccessfully()
    {
        // Arrange
        var moderatorToUpdate = ModeratorHelpers.CreateModerator();
        var newPermissions = new ModeratorPermissions(
            canManageModerators: false,
            canManageContent: true,
            canManageUsers: false,
            canViewReports: true);

        // Act
        var result = _activeModerator.UpdateModeratorPermissions(moderatorToUpdate, newPermissions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        moderatorToUpdate.Permissions.Should().Be(newPermissions);
    }

    [Fact]
    public void UpdateModeratorPermissions_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var moderatorToUpdate = ModeratorHelpers.CreateModerator();
        var newPermissions = ModeratorPermissions.CreateDefault();

        // Act
        var result = _inactiveModerator.UpdateModeratorPermissions(moderatorToUpdate, newPermissions);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }

    [Fact]
    public void UpdateModeratorPermissions_WhenModeratorCannotManageModerators_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator();
            
        var moderatorToUpdate = ModeratorHelpers.CreateModerator();
        var newPermissions = ModeratorPermissions.CreateDefault();

        // Act
        var result = moderatorWithoutPermissions.UpdateModeratorPermissions(moderatorToUpdate, newPermissions);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageModerators);
    }

    [Fact]
    public void UpdateModeratorPermissions_WhenUpdatingSelf_ShouldReturnFailure()
    {
        // Arrange
        var newPermissions = ModeratorPermissions.CreateDefault();

        // Act
        var result = _activeModerator.UpdateModeratorPermissions(_activeModerator, newPermissions);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageHimself);
    }

    [Fact]
    public void UpdateModeratorPermissions_WhenTargetModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var moderatorToUpdate = ModeratorHelpers.CreateModerator(isActive: false);
        var newPermissions = ModeratorPermissions.CreateDefault();

        // Act
        var result = _activeModerator.UpdateModeratorPermissions(moderatorToUpdate, newPermissions);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }

    [Fact]
    public void ActivateModerator_WhenAllConditionsMet_ShouldActivateSuccessfully()
    {
        // Arrange
        var moderatorToActivate = ModeratorHelpers.CreateModerator(isActive: false);
        moderatorToActivate.IsActive.Should().BeFalse();

        // Act
        var result = _activeModerator.ActivateModerator(moderatorToActivate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        moderatorToActivate.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ActivateModerator_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var moderatorToActivate = ModeratorHelpers.CreateModerator(isActive: false);

        // Act
        var result = _inactiveModerator.ActivateModerator(moderatorToActivate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }

    [Fact]
    public void ActivateModerator_WhenModeratorCannotManageModerators_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator();
            
        var moderatorToActivate = ModeratorHelpers.CreateModerator(isActive: false);

        // Act
        var result = moderatorWithoutPermissions.ActivateModerator(moderatorToActivate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageModerators);
    }

    [Fact]
    public void ActivateModerator_WhenActivatingSelf_ShouldReturnFailure()
    {
        // Arrange
        var inactiveSelf = ModeratorHelpers.CreateAdmin(isActive: false);

        // Act
        var result = inactiveSelf.ActivateModerator(inactiveSelf);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageHimself);
    }

    [Fact]
    public void ActivateModerator_WhenTargetModeratorIsAlreadyActive_ShouldReturnFailure()
    {
        // Arrange
        var moderatorToActivate = ModeratorHelpers.CreateModerator();

        // Act
        var result = _activeModerator.ActivateModerator(moderatorToActivate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.AlreadyActive);
    }

    [Fact]
    public void DeactivateModerator_WhenAllConditionsMet_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var moderatorToDeactivate = ModeratorHelpers.CreateModerator(isActive: true);

        // Act
        var result = _activeModerator.DeactivateModerator(moderatorToDeactivate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        moderatorToDeactivate.IsActive.Should().BeFalse();
    }
    
    [Fact]
    public void DeactivateModerator_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var moderatorToActivate = ModeratorHelpers.CreateModerator(isActive: false);

        // Act
        var result = _inactiveModerator.DeactivateModerator(moderatorToActivate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public void DeactivateModerator_WhenModeratorCannotManageModerators_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator(isActive: true);
            
        var moderatorToActivate = ModeratorHelpers.CreateModerator(isActive: false);

        // Act
        var result = moderatorWithoutPermissions.DeactivateModerator(moderatorToActivate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageModerators);
    }

    [Fact]
    public void DeactivateModerator_WhenDeactivatingSelf_ShouldReturnFailure()
    {
        // Act
        var result = _activeModerator.DeactivateModerator(_activeModerator);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageHimself);
    }

    [Fact]
    public void DeactivateModerator_WhenTargetModeratorIsAlreadyInactive_ShouldReturnFailure()
    {
        // Arrange
        var moderatorToDeactivate = ModeratorHelpers.CreateModerator(isActive: false);

        // Act
        var result = _activeModerator.DeactivateModerator(moderatorToDeactivate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.AlreadyDeactivated);
    }
}