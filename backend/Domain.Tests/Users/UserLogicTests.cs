using FluentAssertions;

using Domain.Errors;
using Domain.Events;
using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Tests.Users;

public class UserLogicTests
{
    private readonly Email _testEmail = new("test@example.com");
    private readonly PasswordHash _testPasswordHash = new("TestPassword123!");
    private readonly FilePath _testAvatarPath = new("avatar.jpg");

    [Fact]
    public void Create_ShouldCreateUserWithCorrectProperties()
    {
        // Arrange
        const string fullName = "Test User";

        // Act
        var user = User.Create(_testEmail, _testPasswordHash, fullName);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBe(Guid.Empty);
        user.Email.Should().Be(_testEmail);
        user.PasswordHash.Should().Be(_testPasswordHash);
        user.FullName.Should().Be(fullName);
        user.AvatarPath.Value.Should().Be(string.Empty);
        user.IsActive.Should().BeFalse();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldTrimFullName()
    {
        // Arrange
        const string fullName = "  Test User  ";

        // Act
        var user = User.Create(_testEmail, _testPasswordHash, fullName);

        // Assert
        user.FullName.Should().Be("Test User");
    }
    
    [Fact]
    public void Create_WithNullFullName_ShouldSetNullFullName()
    {
        // Act
        var user = User.Create(_testEmail, _testPasswordHash);

        // Assert
        user.FullName.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldRaiseUserRegisteredEvent()
    {
        // Act
        var user = User.Create(_testEmail, _testPasswordHash);

        // Assert
        user.DomainEvents.Should().ContainSingle(e => e is UserRegisteredEvent);
        var domainEvent = user.DomainEvents.First() as UserRegisteredEvent;
        domainEvent.Should().NotBeNull();
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.Email.Should().Be(_testEmail);
    }

    [Fact]
    public void UpdateProfile_WithNullFullName_ShouldSetNullFullName()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash, "Initial Name");
        user.Activate();

        // Act
        var result = user.UpdateProfile(null, _testAvatarPath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().BeNull();
    }
    
    [Fact]
    public void UpdateProfile_WhenUserIsActive_ShouldUpdateProfileSuccessfully()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
            
        const string newFullName = "Updated Name";
        var newAvatarPath = new FilePath("new_avatar.jpg");

        // Act
        var result = user.UpdateProfile(newFullName, newAvatarPath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be(newFullName);
        user.AvatarPath.Should().Be(newAvatarPath);
    }

    [Fact]
    public void UpdateProfile_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        // User is not activated
        const string newFullName = "Updated Name";

        // Act
        var result = user.UpdateProfile(newFullName, _testAvatarPath);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public void UpdateProfile_WhenAvatarChanged_ShouldRaiseUserAvatarChangedEvent()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
            
        user.UpdateProfile(null, _testAvatarPath);
        var oldAvatarPath = user.AvatarPath;
            
        var newAvatarPath = new FilePath("new_avatar.jpg");
            
        // Act
        var result = user.UpdateProfile("New Name", newAvatarPath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.DomainEvents.Should().Contain(e => e is UserAvatarChangedEvent);
        var domainEvent = user.DomainEvents.OfType<UserAvatarChangedEvent>().First();
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.OldAvatarPath.Should().Be(oldAvatarPath);
    }

    [Fact]
    public void UpdateProfile_WhenAvatarNotChanged_ShouldNotRaiseEvent()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
        user.UpdateProfile("Initial Name", _testAvatarPath);

        // Act
        var result = user.UpdateProfile("Updated Name", _testAvatarPath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.DomainEvents.Should().NotContain(e => e is UserAvatarChangedEvent);
    }
        
    [Fact]
    public void UpdateProfile_WhenUserDoesntHaveAvatarAndSettingNewAvatar_ShouldNotRaiseEvent()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
            
        var newAvatarPath = new FilePath("new_avatar.jpg");
            
        // Act
        var result = user.UpdateProfile("New Name", newAvatarPath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.DomainEvents.Should().NotContain(e => e is UserAvatarChangedEvent);
    }

    [Fact]
    public void UpdateProfile_ShouldTrimFullName()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();

        // Act
        var result = user.UpdateProfile("  Trimmed Name  ", _testAvatarPath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be("Trimmed Name");
    }

    [Fact]
    public void ChangePassword_WhenUserIsActive_ShouldChangePasswordSuccessfully()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
        var newPasswordHash = new PasswordHash("NewPassword123!");

        // Act
        var result = user.ChangePassword(newPasswordHash);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.PasswordHash.Should().Be(newPasswordHash);
    }

    [Fact]
    public void ChangePassword_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        var newPasswordHash = new PasswordHash("NewPassword123!");

        // Act
        var result = user.ChangePassword(newPasswordHash);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public void Activate_WhenUserIsInactive_ShouldActivateSuccessfully()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);

        // Act
        var result = user.Activate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Activate_WhenUserIsAlreadyActive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();

        // Act
        var result = user.Activate();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.UserAlreadyActive);
    }

    [Fact]
    public void Deactivate_WhenUserIsActive_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
        user.UpdateProfile("Test User", _testAvatarPath);

        // Act
        var result = user.Deactivate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        user.AvatarPath.Value.Should().BeEmpty();
    }

    [Fact]
    public void Deactivate_WhenUserIsAlreadyInactive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        // User is not activated

        // Act
        var result = user.Deactivate();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.UserAlreadyDeactivated);
    }
}