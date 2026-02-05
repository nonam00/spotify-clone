using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;
using Application.Moderators.Commands.DeactivateUser;
using Application.Moderators.Errors;
using Application.Users.Errors;

namespace Application.Tests.Moderators.Commands;

public class DeactivateUserCommandHandlerTests : InMemoryTestBase
{
    [Fact] 
    public async Task Handle_ShouldDeactivateUser_WhenValid()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        var user = User.Create(new Email("user@example.com"), new PasswordHash("hashed_password"), "User");
        moderator.ActivateUser(user);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new DeactivateUserCommand(moderator.Id, user.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var deactivatedUser = await Context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == user.Id);
        
        deactivatedUser.Should().NotBeNull();
        deactivatedUser.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldCleanUserRefreshTokensOnDeactivating_WhenValid()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        var user = User.Create(new Email("user@example.com"), new PasswordHash("hashed_password"), "User");
        
        moderator.ActivateUser(user);

        user.AddRefreshToken();
        user.AddRefreshToken();
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new DeactivateUserCommand(moderator.Id, user.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var deactivatedUser = await Context.Users
            .AsNoTracking()
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        
        deactivatedUser.Should().NotBeNull();
        deactivatedUser.IsActive.Should().BeFalse();
        deactivatedUser.RefreshTokens.Count.Should().Be(0);
    }
    
    [Fact] 
    public async Task Handle_ShouldDeleteAvatarImage_WhenValid()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        var user = User.Create(new Email("user@example.com"), new PasswordHash("hashed_password"), "User");
        
        moderator.ActivateUser(user);

        user.UpdateProfile(null, new FilePath("avatar.png"));
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new DeactivateUserCommand(moderator.Id, user.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var deactivatedUser = await Context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == user.Id);
        
        deactivatedUser.Should().NotBeNull();
        deactivatedUser.IsActive.Should().BeFalse();
        deactivatedUser.AvatarPath.Value.Should().Be("");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorNotFound()
    {
        // Arrange
        var command = new DeactivateUserCommand(Guid.NewGuid(), Guid.NewGuid());
        
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
        
        var command = new DeactivateUserCommand(moderator.Id, Guid.NewGuid());
        
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
        
        var command = new DeactivateUserCommand(moderator.Id, Guid.NewGuid());
        
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

        var command = new DeactivateUserCommand(moderator.Id, Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsAlreadyDeactivated()
    {
        // Arrange
        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        var user = User.Create(new Email("user@example.com"), new PasswordHash("hashed_password"), "User");
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new DeactivateUserCommand(moderator.Id, user.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.UserAlreadyDeactivated);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenModeratorIdIsEmpty()
    {
        // Arrange
        var command = new DeactivateUserCommand(Guid.Empty, Guid.NewGuid());

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
        var command = new DeactivateUserCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }
}