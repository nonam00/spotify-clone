using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using FluentAssertions;

using Application.Users.Errors;
using Application.Users.Commands.LoginByRefreshToken;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Commands;

public class LoginByRefreshTokenCommandHandlerTest : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnTokenPair_WhenRefreshTokenIsValid()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var refreshToken = user.AddRefreshToken();
        
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
    public async Task Handle_ShouldReturnTokenPair_WhenRefreshTokenIsNotValid()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var refreshToken = user.AddRefreshToken();
        
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
    public async Task Handle_ShouldReturnTokenPair_WhenRefreshTokenIsExpired()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, DateTime.UtcNow.AddDays(-1));
        
        await Context.Users.AddAsync(user);
        await Context.RefreshTokens.AddAsync(refreshToken);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();

        var query = new LoginByRefreshTokenCommand(refreshTokenValue);
        
        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(RefreshTokenErrors.RelevantNotFound);
    }
}