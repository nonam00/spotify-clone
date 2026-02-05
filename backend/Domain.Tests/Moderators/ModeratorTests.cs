using System.Reflection;
using Domain.Common;
using FluentAssertions;

using Domain.Errors;
using Domain.Models;
using Domain.Tests.Helpers;
using Domain.ValueObjects;

namespace Domain.Tests.Moderators;

public class ModeratorTests
{
    private readonly Email _testEmail = new("moderator@test.com");
    private readonly PasswordHash _testPasswordHash = new("TestPassword123!");
    private readonly ModeratorPermissions _defaultPermissions = ModeratorPermissions.CreateDefault();

    [Fact]
    public void Create_ShouldCreateModeratorWithCorrectProperties()
    {
        // Arrange
        const string fullName = "John Doe";

        // Act
        var moderator = Moderator.Create(_testEmail, _testPasswordHash, fullName, _defaultPermissions);

        // Assert
        moderator.Should().NotBeNull();
        moderator.Id.Should().NotBe(Guid.Empty);
        moderator.Email.Should().Be(_testEmail);
        moderator.PasswordHash.Should().Be(_testPasswordHash);
        moderator.FullName.Should().Be(fullName);
        moderator.IsActive.Should().BeTrue();
        moderator.Permissions.Should().Be(_defaultPermissions);
        moderator.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldTrimFullName()
    {
        // Arrange
        const string fullName = "  John Doe  ";

        // Act
        var moderator = Moderator.Create(_testEmail, _testPasswordHash, fullName, _defaultPermissions);

        // Assert
        moderator.FullName.Should().Be("John Doe");
    }

    [Fact]
    public void Create_WithNullFullName_ShouldSetNullFullName()
    {
        // Act
        var moderator = Moderator.Create(_testEmail, _testPasswordHash, null, _defaultPermissions);

        // Assert
        moderator.FullName.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullPermissions_ShouldUseDefaultPermissions()
    {
        // Act
        var moderator = Moderator.Create(_testEmail, _testPasswordHash, "Test Moderator", null);

        // Assert
        moderator.Permissions.Should().NotBeNull();
        moderator.Permissions.CanManageModerators.Should().BeFalse();
        moderator.Permissions.CanManageContent.Should().BeTrue();
        moderator.Permissions.CanManageUsers.Should().BeTrue();
        moderator.Permissions.CanViewReports.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_PrivateMethod_ShouldDeactivateModerator()
    {
        // Arrange
        var moderator = ModeratorHelpers.CreateModerator();
        
        var deactivateMethod = typeof(Moderator)
            .GetMethod("Deactivate", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = (Result)deactivateMethod!.Invoke(moderator, null)!;

        // Assert
        result.IsSuccess.Should().BeTrue();
        moderator.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_WhenAlreadyDeactivated_ShouldReturnFailure()
    {
        // Arrange
        var moderator = ModeratorHelpers.CreateModerator(isActive: false);
        
        var deactivateMethod = typeof(Moderator)
            .GetMethod("Deactivate", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = (Result)deactivateMethod!.Invoke(moderator, null)!;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.AlreadyDeactivated);
        moderator.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_PrivateMethod_ShouldActivateModerator()
    {
        // Arrange
        var moderator = ModeratorHelpers.CreateModerator(isActive: false);
        
        var activateMethod = typeof(Moderator)
            .GetMethod("Activate", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = (Result)activateMethod!.Invoke(moderator, null)!;

        // Assert
        result.IsSuccess.Should().BeTrue();
        moderator.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldReturnFailure()
    {
        // Arrange
        var moderator = ModeratorHelpers.CreateModerator(isActive: true);

        var activateMethod = typeof(Moderator)
            .GetMethod("Activate", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = (Result)activateMethod!.Invoke(moderator, null)!;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.AlreadyActive);
        moderator.IsActive.Should().BeTrue();
    }
}