using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Users.Queries.LoginByCredentials;
using Application.Users.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Queries;

public class LoginByCredentialsQueryHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnTokenPair_WhenCredentialsAreValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        PasswordHasherMock
            .Setup(x => x.Verify("password", "hashed_password"))
            .Returns(true);
        
        var query = new LoginByCredentialsQuery("test@example.com", "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be("mock_access_token");
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        
        var refreshToken = await Context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.UserId == user.Id);
        
        refreshToken.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var query = new LoginByCredentialsQuery("nonexistent@example.com", "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotActive()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new LoginByCredentialsQuery("test@example.com", "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.AlreadyExistButNotActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        PasswordHasherMock
            .Setup(x => x.Verify("wrongpassword", "hashed_password"))
            .Returns(false);
        
        var query = new LoginByCredentialsQuery("test@example.com", "wrongpassword");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }
}
