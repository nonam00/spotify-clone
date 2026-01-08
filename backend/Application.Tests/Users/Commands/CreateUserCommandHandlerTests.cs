using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

using Application.Users.Commands.CreateUser;
using Application.Users.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Commands;

public class CreateUserCommandHandlerTests : TestBase
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
        user!.FullName.Should().Be("Test User");
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
        user!.FullName.Should().BeNull();
    }
}
