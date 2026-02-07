using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

using Domain.Models;
using Domain.ValueObjects;
using Application.Users.Commands.CreateUser;
using Application.Users.Errors;

namespace Application.Tests.Users.Commands;

public class CreateUserCommandHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_ShouldCreateUser_WhenEmailIsUnique()
    {
        // Arrange
        var command = new CreateUserCommand("test@example.com", "password123", "Test User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var user = await Context.Users.SingleOrDefaultAsync(u => u.Email == "test@example.com");
        
        user.Should().NotBeNull();
        user.FullName.Should().Be("Test User");
        user.IsActive.Should().BeFalse();
        
        PasswordHasherMock.Verify(x => x.Generate("password123"), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExistsAndActive()
    {
        // Arrange
        var existingUser = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Existing User");
        existingUser.Activate();
        
        await Context.Users.AddAsync(existingUser);
        await Context.SaveChangesAsync();
        
        var command = new CreateUserCommand("test@example.com", "password123", "New User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.AlreadyExist);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExistsButNotActive()
    {
        // Arrange
        var existingUser = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Existing User");
        
        await Context.Users.AddAsync(existingUser);
        await Context.SaveChangesAsync();
        
        var command = new CreateUserCommand("test@example.com", "password123", "New User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.AlreadyExistButNotActive);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenFullNameIsNull()
    {
        // Arrange
        var command = new CreateUserCommand("test@example.com", "password123", null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var user = await Context.Users.SingleOrDefaultAsync(u => u.Email == "test@example.com");
        
        user.Should().NotBeNull();
        user.FullName.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new CreateUserCommand("", "password123", "Test User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateUserCommand("invalid-email", "password123", "Test User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailExceedsMaxLength()
    {
        // Arrange
        var longEmail = new string('a', 244) + "@example.com";
        var command = new CreateUserCommand(longEmail, "password123", "Test User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        // Arrange
        var command = new CreateUserCommand("test@example.com", "", "Test User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Password");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = new CreateUserCommand("test@example.com", "short", "Test User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

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
        var command = new CreateUserCommand("test@example.com", longPassword, "Test User");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Password");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 256);
        var command = new CreateUserCommand("test@example.com", "password123", longName);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("FullName");
    }
}
