using Application.Moderators.Commands.DeactivateModerator;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Commands;

public class DeactivateModeratorCommandHandlerTest : TestBase
{
    [Fact] 
    public async Task Handle_ShouldActivateModerator_WhenValid()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        var moderatorToDeactivate = Moderator.Create(
            new Email("toupdate@example.com"),
            new PasswordHash("hashed_password2"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToDeactivate);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new DeactivateModeratorCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToDeactivateId: moderatorToDeactivate.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var deactivatedModerator = await Context.Moderators
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == moderatorToDeactivate.Id);
        
        deactivatedModerator.Should().NotBeNull();
        deactivatedModerator.IsActive.Should().BeFalse();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorDoesNotHavePermissions()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateDefault());
        
        var moderatorToDeactivate = Moderator.Create(
            new Email("toupdate@example.com"),
            new PasswordHash("hashed_password2"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToDeactivate);
        await Context.SaveChangesAsync();
        
        var command = new DeactivateModeratorCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToDeactivateId: moderatorToDeactivate.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageModerators);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorTryingToActivateThemself()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        await Context.Moderators.AddRangeAsync(moderator);
        await Context.SaveChangesAsync();

        var command = new DeactivateModeratorCommand(
            ManagingModeratorId: moderator.Id,
            ModeratorToDeactivateId: moderator.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorIsAlreadyDeactivated()
    {
        
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        var moderatorToDeactivate = Moderator.Create(
            new Email("toupdate@example.com"),
            new PasswordHash("hashed_password2"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        managingModerator.DeactivateModerator(moderatorToDeactivate);
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToDeactivate);
        await Context.SaveChangesAsync();

        var command = new DeactivateModeratorCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToDeactivateId: moderatorToDeactivate.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.AlreadyDeactivated);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var command = new DeactivateModeratorCommand(
            Guid.NewGuid(),
            Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }
}