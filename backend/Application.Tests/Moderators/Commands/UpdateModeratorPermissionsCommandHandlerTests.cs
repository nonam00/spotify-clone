using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Application.Moderators.Commands.UpdateModeratorPermissions;
using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Commands;

public class UpdateModeratorPermissionsCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUpdatePermissions_WhenModeratorExists()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        var moderatorToUpdate = Moderator.Create(
            new Email("toupdate@example.com"),
            new PasswordHash("hashed_password2"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToUpdate);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new UpdateModeratorPermissionsCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToUpdateId: moderatorToUpdate.Id,
            CanManageUsers: true,
            CanManageContent: true,
            CanViewReports: false,
            CanManageModerators: false);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedModerator = await Context.Moderators
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == moderatorToUpdate.Id);
        
        updatedModerator.Should().NotBeNull();
        updatedModerator.Permissions.CanManageUsers.Should().BeTrue();
        updatedModerator.Permissions.CanManageContent.Should().BeTrue();
        updatedModerator.Permissions.CanViewReports.Should().BeFalse();
        updatedModerator.Permissions.CanManageModerators.Should().BeFalse();
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
        
        var moderatorToUpdate = Moderator.Create(
            new Email("toupdate@example.com"),
            new PasswordHash("hashed_password2"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToUpdate);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new UpdateModeratorPermissionsCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToUpdateId: moderatorToUpdate.Id,
            CanManageUsers: true,
            CanManageContent: true,
            CanViewReports: false,
            CanManageModerators: false);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageModerators);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var command = new UpdateModeratorPermissionsCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CanManageUsers: true,
            CanManageContent: true,
            CanViewReports: false,
            CanManageModerators: false);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }
}
