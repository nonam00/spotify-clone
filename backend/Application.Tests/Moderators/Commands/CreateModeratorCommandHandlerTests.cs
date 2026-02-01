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
    public async Task Handle_ShouldCreateModerator_WhenValid()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new CreateModeratorCommand(
            managingModerator.Id,
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
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
                
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new CreateModeratorCommand(
            managingModerator.Id,
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
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorDoesNotHavePermissions()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateDefault());
                
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new CreateModeratorCommand(
            managingModerator.Id,
            "admin@example.com",
            "Admin Name",
            "password123",
            true);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageModerators);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorIsNotActive()
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
            ModeratorPermissions.CreateSuperAdmin());

        admin.DeactivateModerator(managingModerator);
        
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new CreateModeratorCommand(
            managingModerator.Id,
            "admin@example.com",
            "Admin Name",
            "password123",
            true);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExistsAndActive()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        var existingModerator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password"),
            "Existing Moderator",
            ModeratorPermissions.CreateDefault());
        // Already active after creation
        
        await Context.Moderators.AddRangeAsync(managingModerator, existingModerator);
        await Context.SaveChangesAsync();
        
        var command = new CreateModeratorCommand(
            managingModerator.Id,
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
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        var existingModerator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password"),
            "Existing Moderator",
            ModeratorPermissions.CreateDefault());
            
        managingModerator.DeactivateModerator(existingModerator); // Deactivate the moderator
        
        await Context.Moderators.AddRangeAsync(managingModerator, existingModerator);
        await Context.SaveChangesAsync();
        
        var command = new CreateModeratorCommand(
            managingModerator.Id,
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
