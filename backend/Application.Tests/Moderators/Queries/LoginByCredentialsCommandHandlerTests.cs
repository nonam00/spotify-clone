using FluentAssertions;

using Application.Moderators.Queries.LoginByCredentials;
using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Queries;

public class LoginByCredentialsCommandHandlerTests : TestBase
{
        [Fact]
    public async Task Handle_ShouldReturnAccessToken_WhenCredentialsAreValid()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test Moderator");
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        PasswordHasherMock
            .Setup(x => x.Verify("password", "hashed_password"))
            .Returns(true);
        
        var query = new LoginByCredentialsQuery("test@example.com", "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("mock_moderator_token");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var query = new LoginByCredentialsQuery("nonexistent@example.com", "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotActive()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        moderator.Deactivate();
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        var query = new LoginByCredentialsQuery("test@example.com", "password");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.AlreadyExistButNotActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        moderator.Activate();
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        PasswordHasherMock
            .Setup(x => x.Verify("wrongpassword", "hashed_password"))
            .Returns(false);
        
        var query = new LoginByCredentialsQuery("test@example.com", "wrongpassword");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.InvalidCredentials);
    }
}