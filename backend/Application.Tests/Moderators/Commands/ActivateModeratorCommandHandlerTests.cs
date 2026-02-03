using Microsoft.EntityFrameworkCore;
using FluentAssertions;

using Application.Moderators.Commands.ActivateModerator;
using Application.Moderators.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Moderators.Commands;

public class ActivateModeratorCommandHandlerTests : TestBase
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
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new ActivateModeratorCommand(
            ManagingModeratorId: managingModerator.Id,
            ModeratorToActivateId: Guid.NewGuid());
        
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
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorNotFound()
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
        
        var command = new ActivateModeratorCommand(
            managingModerator.Id,
            Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorToActivateNotFound()
    {
        // Arrange
        var managingModerator = Moderator.Create(
            new Email("managing@example.com"),
            new PasswordHash("hashed_password1"),
            "Managing Moderator",
            ModeratorPermissions.CreateSuperAdmin());
        
        await Context.Moderators.AddAsync(managingModerator);
        await Context.SaveChangesAsync();
        
        var command = new ActivateModeratorCommand(
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
        var command = new ActivateModeratorCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ManagingModeratorId");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenModeratorToActivateIdIsEmpty()
    {
        // Arrange
        var command = new ActivateModeratorCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ModeratorToActivateId");
    }
}