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

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailIsEmpty()
    {
        // Arrange
        var query = new LoginByCredentialsQuery("", "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailIsInvalid()
    {
        // Arrange
        var query = new LoginByCredentialsQuery("invalid-email", "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailExceedsMaxLength()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com";
        var query = new LoginByCredentialsQuery(longEmail, "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        // Arrange
        var query = new LoginByCredentialsQuery("test@example.com", "");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Password");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPasswordExceedsMaxLength()
    {
        // Arrange
        var longPassword = new string('a', 101);
        var query = new LoginByCredentialsQuery("test@example.com", longPassword);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Password");
    }
}
