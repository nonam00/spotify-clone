using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Domain.ValueObjects;
using Application.Users.Errors;
using Application.Users.Commands.LoginByRefreshToken;

namespace Application.Tests.Users.Commands;

public class LoginByRefreshTokenCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnTokenPair_WhenRefreshTokenIsValid()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var refreshToken = user.AddRefreshToken().Value;
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();

        var query = new LoginByRefreshTokenCommand(refreshToken.Token);
        
        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("mock_access_token");
        result.Value.RefreshToken.Should().NotBe(refreshToken.Token);
        
        var userWithTokens = await Context.Users
            .AsNoTracking()
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Id == user.Id);
        
        userWithTokens.Should().NotBeNull();
        userWithTokens.RefreshTokens.Count.Should().Be(1);
        userWithTokens.RefreshTokens.First().Should().NotBe(refreshToken.Token);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRefreshTokenIsNotValid()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        user.AddRefreshToken();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();

        var query = new LoginByRefreshTokenCommand("8391831983");
        
        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(RefreshTokenErrors.RelevantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotActive()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var refreshToken = user.AddRefreshToken().Value;

        var moderator = Moderator.Create(new Email("mod@mail.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.DeactivateUser(user);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();

        var query = new LoginByRefreshTokenCommand(refreshToken.Token);
        
        // Act
        var result = await Mediator.Send(query, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }
    
    // [Fact] public async Task Handle_ShouldReturnFailure_WhenRefreshTokenIsExpired() { }
}