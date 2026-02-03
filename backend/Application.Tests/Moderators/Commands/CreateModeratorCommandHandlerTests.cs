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
    public async Task Handle_ShouldReturnFailure_WhenManagingModeratorNotFound()
    {
        // Arrange
        var command = new CreateModeratorCommand(
            Guid.NewGuid(), 
            "admin@example.com",
            "Admin Name",
            "password123",
            true);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }
    
        [Fact]
    public async Task Handle_ShouldReturnValidationFailure_WhenFullNameIsEmpty()
    {
        // Arrange
        var command = new CreateModeratorCommand(
            Guid.NewGuid(),
            "test@example.com",
            "",
            "password123",
            false);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("FullName");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new CreateModeratorCommand(
            Guid.NewGuid(),
            "",
            "Moderator",
            "password123",
            false);
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateModeratorCommand(
            Guid.NewGuid(),
            "invalid-email",
            "Moderator",
            "password123",
            false);
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenEmailExceedsMaxLength()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com";
        var command = new CreateModeratorCommand(
            Guid.NewGuid(),
            longEmail,
            "Moderator",
            "password123",
            false);
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPasswordIsEmpty()
    {
        // Arrange
        var command = new CreateModeratorCommand(
            Guid.NewGuid(), 
            "admin@example.com",
            "Admin Name",
            "",
            false);
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Password");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = new CreateModeratorCommand(
            Guid.NewGuid(), 
            "admin@example.com",
            "Admin Name",
            "pass",
            true);
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Password");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPasswordExceedsMaxLength()
    {
        // Arrange
        var longPassword = new string('a', 101);
        var command = new CreateModeratorCommand(
            Guid.NewGuid(), 
            "admin@example.com",
            "Admin Name",
            longPassword,
            true);
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Password");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new CreateModeratorCommand(
            Guid.NewGuid(), 
            "admin@example.com",
            longName,
            "password123",
            true);
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("FullName");
    }
}
