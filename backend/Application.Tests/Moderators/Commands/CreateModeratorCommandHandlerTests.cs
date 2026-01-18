using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

using Application.Moderators.Commands.CreateModerator;
using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Commands;

public class CreateModeratorCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldCreateModerator_WhenEmailIsUnique()
    {
        // Arrange
        var command = new CreateModeratorCommand(
            "moderator@example.com",
            "Moderator Name", 
            "password123",
            false);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var moderator = await Context.Moderators.FirstOrDefaultAsync(m => m.Email == "moderator@example.com");
        
        moderator.Should().NotBeNull();
        moderator.IsActive.Should().BeTrue();
        moderator.Permissions.CanManageModerators.Should().BeFalse();
        
        PasswordHasherMock.Verify(x => x.Generate("password123"), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateSuperAdmin_WhenIsSuperIsTrue()
    {
        // Arrange
        var command = new CreateModeratorCommand(
            "admin@example.com",
            "Admin Name",
            "password123",
            true);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var moderator = await Context.Moderators.FirstOrDefaultAsync(m => m.Email == "admin@example.com");
        
        moderator.Should().NotBeNull();
        moderator.IsActive.Should().BeTrue();
        moderator.Permissions.CanManageModerators.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExistsAndActive()
    {
        // Arrange
        var existingModerator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password"),
            "Existing Moderator",
            ModeratorPermissions.CreateDefault());
        // Already active after creation
        
        await Context.Moderators.AddAsync(existingModerator);
        await Context.SaveChangesAsync();
        
        var command = new CreateModeratorCommand(
            "moderator@example.com",
            "New Moderator",
            "password123",
            false);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.AlreadyExist);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExistsButNotActive()
    {
        // Arrange
        var existingModerator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password"),
            "Existing Moderator",
            ModeratorPermissions.CreateDefault());
        existingModerator.Deactivate(); // Deactivate the moderator
        
        await Context.Moderators.AddAsync(existingModerator);
        await Context.SaveChangesAsync();
        
        var command = new CreateModeratorCommand(
            "moderator@example.com", 
            "New Moderator",
            "password123",
            false);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.AlreadyExistButNotActive);
    }
}
