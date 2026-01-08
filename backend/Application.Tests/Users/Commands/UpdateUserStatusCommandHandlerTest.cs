using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Users.Commands.UpdateUserStatus;
using Application.Users.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Commands;

public class UpdateUserStatusCommandHandlerTest : TestBase
{
    [Fact]
    public async Task Handle_ShouldActivateUser_WhenValidAndTryingToActivate()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdateUserStatusCommand(user.Id, true);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedUser = await Context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == user.Id);
        
        updatedUser.Should().NotBeNull();
        updatedUser!.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldDeactivateUser_WhenValidAndTryingToDeactivate()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdateUserStatusCommand(user.Id, false);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedUser = await Context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == user.Id);
        
        updatedUser.Should().NotBeNull();
        updatedUser!.IsActive.Should().BeFalse();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdateUserStatusCommand(Guid.NewGuid(), false);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }
}