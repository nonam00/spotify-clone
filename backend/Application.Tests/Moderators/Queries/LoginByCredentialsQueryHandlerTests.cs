using FluentAssertions;

using Application.Moderators.Queries.LoginByCredentials;
using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Queries;

public class LoginByCredentialsQueryHandlerTests : InMemoryTestBase
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
        var admin = Moderator.Create(
            new Email("admin@example.com"),
            new PasswordHash("hashed_password_admin"),
            "Admin",
            ModeratorPermissions.CreateSuperAdmin());
        
        var moderator = Moderator.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        
        admin.DeactivateModerator(moderator);
        
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