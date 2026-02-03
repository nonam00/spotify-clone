using Microsoft.EntityFrameworkCore;
using FluentAssertions;

using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;
using Application.Moderators.Errors;
using Application.Moderators.Commands.DeactivateModerator;

namespace Application.Tests.Moderators.Commands;

public class DeactivateModeratorCommandHandlerTests : TestBase
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
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorNotFound()
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
        
        var command = new DeactivateModeratorCommand(
            managingModerator.Id,
            Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorToDeactivateNotFound()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new DeactivateModeratorCommand(
            managingModerator.Id,
            Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenManagingModeratorIdIsEmpty()
    {
        // Arrange
        var command = new DeactivateModeratorCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ManagingModeratorId");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenModeratorToDeactivateIdIsEmpty()
    {
        // Arrange
        var command = new DeactivateModeratorCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ModeratorToDeactivateId");
    }
}