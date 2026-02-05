using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;
using Application.Users.Commands.CleanUserRefreshTokens;
using Application.Users.Errors;

namespace Application.Tests.Users.Commands;

public class CleanUserRefreshTokensCommandHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_ShouldCleanUserRefreshTokens_WhenValid()
    {
        // Arrange
        var user = User.Create(new Email("mail@example.com"), new PasswordHash("hashed_password"));
        user.Activate();
        
        user.AddRefreshToken();
        user.AddRefreshToken();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new CleanUserRefreshTokensCommand(user.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var userRefreshTokens = await Context.RefreshTokens
            .AsNoTracking()
            .Where(rf => rf.UserId == user.Id)
            .ToListAsync(CancellationToken.None);

        userRefreshTokens.Count.Should().Be(0);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new CleanUserRefreshTokensCommand(Guid.NewGuid());
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Task_ShouldReturnFailure_WhenUserIsNotActive()
    {
        // Arrange
        var user = User.Create(new Email("mail@example.com"), new PasswordHash("hashed_password"));

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new CleanUserRefreshTokensCommand(user.Id);
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationFailure_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new CleanUserRefreshTokensCommand(Guid.Empty);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }
}