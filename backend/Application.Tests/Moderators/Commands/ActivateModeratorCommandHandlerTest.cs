using Microsoft.EntityFrameworkCore;
using FluentAssertions;

using Application.Moderators.Commands.ActivateModerator;
using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Commands;

public class ActivateModeratorCommandHandlerTest : TestBase
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
        
        var moderatorToActivate = Moderator.Create(
            new Email("toupdate@example.com"),
            new PasswordHash("hashed_password2"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        managingModerator.DeactivateModerator(moderatorToActivate);
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToActivate);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new ActivateModeratorCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToActivateId: moderatorToActivate.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var activatedModerator = await Context.Moderators
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == moderatorToActivate.Id);
        
        activatedModerator.Should().NotBeNull();
        activatedModerator.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorDoesNotHavePermissions()
    {
        // Arrange
        var admin = Moderator.Create(
            new Email("admin@example.com"),
            new PasswordHash("hashed_password_admin"),
            "Admin",
            ModeratorPermissions.CreateSuperAdmin());
        
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateDefault());
        
        var moderatorToActivate = Moderator.Create(
            new Email("toupdate@example.com"),
            new PasswordHash("hashed_password2"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        admin.DeactivateModerator(moderatorToActivate);
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToActivate);
        await Context.SaveChangesAsync();
        
        var command = new ActivateModeratorCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToActivateId: moderatorToActivate.Id);
        
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

        var command = new ActivateModeratorCommand(
            ManagingModeratorId: moderator.Id,
            ModeratorToActivateId: moderator.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorIsAlreadyActive()
    {
        
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        var moderatorToActivate = Moderator.Create(
            new Email("toupdate@example.com"),
            new PasswordHash("hashed_password2"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToActivate);
        await Context.SaveChangesAsync();

        var command = new ActivateModeratorCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToActivateId: moderatorToActivate.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.AlreadyActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var command = new ActivateModeratorCommand(
            Guid.NewGuid(),
            Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }
}