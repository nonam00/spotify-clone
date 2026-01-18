using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Moderators.Commands.UpdateModeratorStatus;
using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Commands;

public class UpdateModeratorStatusCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldActivateModerator_WhenModeratorIsNotActiveAndTryingToActivate()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password"),
            "Moderator Name",
            ModeratorPermissions.CreateDefault());
        moderator.Deactivate();
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdateModeratorStatusCommand(moderator.Id, true);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedModerator = await Context.Moderators.FirstOrDefaultAsync(m => m.Id == moderator.Id);

        updatedModerator.Should().NotBeNull();
        updatedModerator.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldDeactivateModerator_WhenModeratorIsActiveAndTryingToDeactivate()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password"),
            "Moderator Name",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdateModeratorStatusCommand(moderator.Id, false);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedModerator = await Context.Moderators.FirstOrDefaultAsync(m => m.Id == moderator.Id);
        
        updatedModerator.Should().NotBeNull();
        updatedModerator.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var command = new UpdateModeratorStatusCommand(Guid.NewGuid(), true);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }
    
    // TODO: active -> activate and not active -> deactivate 
}
