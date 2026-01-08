using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Users.Commands.UpdatePassword;
using Application.Users.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Commands;

public class UpdatePasswordCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUpdatePassword_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_oldpassword"),
            "Test User");
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
        updatedUser!.PasswordHash.Value.Should().Be("hashed_newpassword");
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
    public async Task Handle_ShouldReturnFailure_WhenCurrentPasswordIsIncorrect()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_oldpassword"),
            "Test User");
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
}
