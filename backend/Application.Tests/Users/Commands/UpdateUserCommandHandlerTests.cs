using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Users.Commands.UpdateUser;
using Application.Users.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Commands;

public class UpdateUserCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Old Name");
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
        updatedUser!.FullName.Should().Be("New Name");
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
    public async Task Handle_ShouldUpdateOnlyFullName_WhenAvatarPathIsNull()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Old Name");
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
        updatedUser!.FullName.Should().Be("New Name");
        updatedUser.AvatarPath.Value.Should().Be(originalAvatarPath);
    }
}
