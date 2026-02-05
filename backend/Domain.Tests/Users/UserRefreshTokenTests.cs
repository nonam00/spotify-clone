using FluentAssertions;

using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Tests.Users;

public class UserRefreshTokenTests
{
    private readonly Email _testEmail = new("test@example.com");
    private readonly PasswordHash _testPasswordHash = new("TestPassword123!");
    
    [Fact]
    public void AddRefreshToken_WhenUserIsActive_ShouldAddTokenSuccessfully()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();

        // Act
        var result = user.AddRefreshToken();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        user.RefreshTokens.Should().Contain(result.Value);
        result.Value.UserId.Should().Be(user.Id);
        result.Value.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public void AddRefreshToken_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        // User is not activated

        // Act
        var result = user.AddRefreshToken();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
        user.RefreshTokens.Should().BeEmpty();
    }

    [Fact]
    public void UpdateRefreshToken_WhenTokenExists_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
        var addTokenResult = user.AddRefreshToken();
        var oldTokenValue = addTokenResult.Value.Token;

        // Act
        var result = user.UpdateRefreshToken(oldTokenValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().NotBe(oldTokenValue);
        user.RefreshTokens.Should().Contain(result.Value);
    }

    [Fact]
    public void UpdateRefreshToken_WhenTokenDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();

        // Act
        var result = user.UpdateRefreshToken("nonexistent-token");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.RefreshTokenNotFound);
    }

    [Fact]
    public void UpdateRefreshToken_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        // User is not activated

        // Act
        var result = user.UpdateRefreshToken("any-token");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public void CleanRefreshTokens_WhenUserIsActive_ShouldClearTokensSuccessfully()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
            
        user.AddRefreshToken();
        user.AddRefreshToken();
            
        // Act
        var result = user.CleanRefreshTokens();

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.RefreshTokens.Should().BeEmpty();
    }

    [Fact]
    public void CleanRefreshTokens_WhenUserIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        // User is not activated

        // Act
        var result = user.CleanRefreshTokens();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public void Deactivate_ShouldClearRefreshTokens()
    {
        // Arrange
        var user = User.Create(_testEmail, _testPasswordHash);
        user.Activate();
            
        user.AddRefreshToken();
        user.AddRefreshToken();

        // Act
        var result = user.Deactivate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.RefreshTokens.Should().BeEmpty();
    }
}