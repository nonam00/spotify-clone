using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Application.Moderators.Commands.UpdateModeratorPermissions;
using Application.Moderators.Errors;
using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Commands;

public class UpdateModeratorPermissionsCommandHandlerTests : InMemoryTestBase
{
    [Fact] 
    public async Task Handle_ShouldUpdatePermissions_WhenValid()
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
            ModeratorToUpdatePermissionsId: moderatorToUpdate.Id,
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
        
        var command = new UpdateModeratorPermissionsCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToUpdatePermissionsId: moderatorToUpdate.Id,
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
    public async Task Handle_ShouldReturnFailure_WhenModeratorTryingToUpdateSelfPermissions()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        await Context.Moderators.AddRangeAsync(moderator);
        await Context.SaveChangesAsync();

        var command = new UpdateModeratorPermissionsCommand(
            ManagingModeratorId: moderator.Id,
            ModeratorToUpdatePermissionsId: moderator.Id,
            CanManageUsers: true,
            CanManageContent: true,
            CanViewReports: false,
            CanManageModerators: true);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorNotFound()
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
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorIsNotActive()
    {
        // Arrange
        var admin = Moderator.Create(
            new Email("admin@example.com"),
            new PasswordHash("admin_hash"),
            "Admin",
            ModeratorPermissions.CreateSuperAdmin());
        
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        admin.DeactivateModerator(managingModerator);
        
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new UpdateModeratorPermissionsCommand(
            managingModerator.Id,
            Guid.NewGuid(),
            CanManageUsers: true,
            CanManageContent: true,
            CanViewReports: false,
            CanManageModerators: false);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorToUpdateNotFound()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new UpdateModeratorPermissionsCommand(
            managingModerator.Id,
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

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorToUpdateIsNotActive()
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
        
        managingModerator.DeactivateModerator(moderatorToUpdate);
        
        await Context.Moderators.AddRangeAsync(managingModerator, moderatorToUpdate);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new UpdateModeratorPermissionsCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToUpdatePermissionsId: moderatorToUpdate.Id,
            CanManageUsers: true,
            CanManageContent: true,
            CanViewReports: false,
            CanManageModerators: false);
        
        // Act 
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
}
