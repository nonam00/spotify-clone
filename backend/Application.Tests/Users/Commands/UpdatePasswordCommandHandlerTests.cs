using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Domain.ValueObjects;
using Application.Users.Commands.UpdatePassword;
using Application.Users.Errors;
using Domain.Errors;

namespace Application.Tests.Users.Commands;

public class UpdatePasswordCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUpdatePassword_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_oldpassword"), "User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        PasswordHasherMock
            .Setup(x => x.Verify("oldpassword", "hashed_oldpassword"))
            .Returns(true);
        PasswordHasherMock
            .Setup(x => x.Generate("newpassword"))
            .Returns("hashed_newpassword");
        
        var command = new UpdatePasswordCommand(user.Id, "oldpassword", "newpassword");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedUser = await Context.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
        
        updatedUser.Should().NotBeNull();
        updatedUser.PasswordHash.Value.Should().Be("hashed_newpassword");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "oldpassword", "newpassword");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotActive()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_oldpassword"), "User");

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new UpdatePasswordCommand(user.Id, "oldpassword", "newpassword");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCurrentPasswordIsIncorrect()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_oldpassword"), "User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        PasswordHasherMock.Setup(x => x.Verify("wrongpassword", "hashed_oldpassword"))
            .Returns(false);
        
        var command = new UpdatePasswordCommand(user.Id, "wrongpassword", "newpassword");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.PasswordsMissMatch);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCurrentPasswordIsEmpty()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "", "newpassword123");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("CurrentPassword");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCurrentPasswordIsTooShort()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "short", "newpassword123");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("CurrentPassword");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCurrentPasswordExceedsMaxLength()
    {
        // Arrange
        var longPassword = new string('a', 101);
        var command = new UpdatePasswordCommand(Guid.NewGuid(), longPassword, "newpassword123");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("CurrentPassword");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNewPasswordIsEmpty()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "oldpassword123", "");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("NewPassword");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNewPasswordIsTooShort()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "oldpassword123", "short");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("NewPassword");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNewPasswordExceedsMaxLength()
    {
        // Arrange
        var longPassword = new string('a', 101);
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "oldpassword123", longPassword);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("NewPassword");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNewPasswordEqualsCurrentPassword()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "password123", "password123");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("New password must be different");
    }
}