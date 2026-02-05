using FluentAssertions;

using Domain.Errors;
using Domain.Events;
using Domain.Models;
using Domain.Tests.Helpers;
using Domain.ValueObjects;

namespace Domain.Tests.Moderators;

public class ModeratorUserTests
{
    private readonly Moderator _activeModerator = ModeratorHelpers.CreateModerator(isActive: true);
    private readonly Moderator _inactiveModerator = ModeratorHelpers.CreateModerator(isActive: false);


    [Fact]
    public void ActivateUser_WhenAllConditionsMet_ShouldActivateUser()
    {
        // Arrange
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));
        user.IsActive.Should().BeFalse();

        // Act
        var result = _activeModerator.ActivateUser(user);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ActivateUser_WhenModeratorCannotManageUsers_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator(
            canViewReports: true,
            canManageContent: true,
            canManageUsers: false);
            
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));

        // Act
        var result = moderatorWithoutPermissions.ActivateUser(user);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageUsers);
    }

    [Fact]
    public void ActivateUser_WhenUserIsAlreadyActive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));
        user.Activate();

        // Act
        var result = _activeModerator.ActivateUser(user);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.UserAlreadyActive);
    }
    
    [Fact]
    public void ActivateUser_WhenModeratorIsNotActive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));
        
        // Act
        var result =  _inactiveModerator.ActivateUser(user);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }


    [Fact]
    public void DeactivateUser_WhenAllConditionsMet_ShouldDeactivateUser()
    {
        // Arrange
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));
            
        var avatarPath = new FilePath("user.jpg");
        user.Activate();
        user.UpdateProfile("Test User", avatarPath);

        _activeModerator.CleanDomainEvents();

        // Act
        var result = _activeModerator.DeactivateUser(user);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        _activeModerator.DomainEvents.Should().ContainSingle(e => e is ModeratorDeactivatedUserEvent);
            
        var domainEvent = _activeModerator.DomainEvents.OfType<ModeratorDeactivatedUserEvent>().First();
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.AvatarPath.Should().Be(avatarPath);
    }

    [Fact]
    public void DeactivateUser_WhenUserHasNoAvatar_ShouldNotRaiseEvent()
    {
        // Arrange
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));
            
        user.Activate();
        user.UpdateProfile("Test User", new FilePath(null));
        user.IsActive.Should().BeTrue();

        _activeModerator.CleanDomainEvents();

        // Act
        var result = _activeModerator.DeactivateUser(user);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        _activeModerator.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DeactivateUser_WhenUserIsAlreadyInactive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));

        // Act
        var result = _activeModerator.DeactivateUser(user);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.UserAlreadyDeactivated);
    }

    [Fact]
    public void DeactivateUser_WhenModeratorIsNotActive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));

        user.Activate();
        
        // Act
        var result =  _inactiveModerator.DeactivateUser(user);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }

    [Fact]
    public void DeactivateUser_WhenModeratorCannotManageUsers_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator(
            canViewReports: true,
            canManageContent: true,
            canManageUsers: false);
            
        var user = User.Create(
            new Email("testuser@test.com"),
            new PasswordHash("UserPassword123!"));
            
        user.Activate();

        // Act
        var result = moderatorWithoutPermissions.DeactivateUser(user);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageUsers);
    }
}