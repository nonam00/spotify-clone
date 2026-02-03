using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Domain.ValueObjects;
using Application.Users.Commands.UpdateUser;
using Application.Users.Errors;
using Domain.Errors;

namespace Application.Tests.Users.Commands;

public class UpdateUserCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "Old Name");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdateUserCommand(user.Id, "New Name", "new_avatar.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedUser = await Context.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
        
        updatedUser.Should().NotBeNull();
        updatedUser.FullName.Should().Be("New Name");
        updatedUser.AvatarPath.Value.Should().Be("new_avatar.jpg");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "New Name", "avatar.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotActive()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "Old Name");

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new UpdateUserCommand(user.Id, "New Name", "avatar.jpg");
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public async Task Handle_ShouldUpdateOnlyFullName_WhenAvatarPathIsNull()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "Old Name");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var originalAvatarPath = user.AvatarPath.Value;
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdateUserCommand(user.Id, "New Name", null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedUser = await Context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == user.Id);
        
        updatedUser.Should().NotBeNull();
        updatedUser.FullName.Should().Be("New Name");
        updatedUser.AvatarPath.Value.Should().Be(originalAvatarPath);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.Empty, "New Name", "avatar.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new UpdateUserCommand(Guid.NewGuid(), longName, null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("FullName");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenAvatarPathExceedsMaxLength()
    {
        // Arrange
        var longPath = new string('a', 501);
        var command = new UpdateUserCommand(Guid.NewGuid(), null, longPath);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("AvatarPath");
    }
}
