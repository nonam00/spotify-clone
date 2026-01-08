using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Application.Moderators.Commands.UpdateModeratorPermissions;
using Application.Moderators.Errors;
using Application.Shared.Messaging;
using Application.Shared.Data;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Commands;

public class UpdateModeratorPermissionsCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUpdatePermissions_WhenModeratorExists()
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

        var command = new UpdateModeratorPermissionsCommand(
            moderator.Id,
            CanManageUsers: true,
            CanManageContent: true,
            CanViewReports: false,
            CanManageModerators: false);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedModerator = await Context.Moderators.FirstOrDefaultAsync(m => m.Id == moderator.Id);
        
        updatedModerator.Should().NotBeNull();
        updatedModerator!.Permissions.CanManageUsers.Should().BeTrue();
        updatedModerator.Permissions.CanManageContent.Should().BeTrue();
        updatedModerator.Permissions.CanViewReports.Should().BeFalse();
        updatedModerator.Permissions.CanManageModerators.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var command = new UpdateModeratorPermissionsCommand(
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
