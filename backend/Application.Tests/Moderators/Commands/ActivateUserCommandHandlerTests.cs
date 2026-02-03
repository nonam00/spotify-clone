using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Domain.ValueObjects;
using Application.Moderators.Commands.ActivateUser;
using Application.Moderators.Errors;
using Application.Users.Errors;

namespace Application.Tests.Moderators.Commands;

public class ActivateUserCommandHandlerTests : TestBase
{
    [Fact] 
    public async Task Handle_ShouldActivateUser_WhenValid()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        var user = User.Create(new Email("user@example.com"), new PasswordHash("hashed_password"), "User");

        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new ActivateUserCommand(moderator.Id, user.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var activatedUser = await Context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == user.Id);
        
        activatedUser.Should().NotBeNull();
        activatedUser.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var command = new ActivateUserCommand(Guid.NewGuid(), Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorIsNotActive()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        
        var admin = Moderator.Create(
            new Email("admin@example.com"),
            new PasswordHash("admin_hash"),
            "Admin",
            ModeratorPermissions.CreateSuperAdmin());
        admin.DeactivateModerator(moderator);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        var command = new ActivateUserCommand(moderator.Id, Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorDoesNotHavePermissions()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        
        var permissions = new ModeratorPermissions(
            canManageContent: true,
            canManageUsers: false,
            canViewReports: false,
            canManageModerators: false);
        
        var admin = Moderator.Create(
            new Email("admin@example.com"),
            new PasswordHash("admin_hash"),
            "Admin",
            ModeratorPermissions.CreateSuperAdmin());
        
        admin.UpdateModeratorPermissions(moderator, permissions);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        var command = new ActivateUserCommand(moderator.Id, Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageUsers);
    }
        
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();

        var command = new ActivateUserCommand(moderator.Id, Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsAlreadyActive()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        var user = User.Create(new Email("user@example.com"), new PasswordHash("hashed_password"), "User");

        moderator.ActivateUser(user);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new ActivateUserCommand(moderator.Id, user.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.UserAlreadyActive);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenModeratorIdIsEmpty()
    {
        // Arrange
        var command = new ActivateUserCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ModeratorId");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new ActivateUserCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }
}