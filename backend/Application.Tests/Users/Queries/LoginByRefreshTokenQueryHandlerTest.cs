using System.Security.Cryptography;
using Application.Users.Errors;
using Application.Users.Queries.LoginByRefreshToken;
using Domain.Models;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests.Users.Queries;

public class LoginByRefreshTokenQueryHandlerTest : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnTokenPair_WhenRefreshTokenIsValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        user.AddRefreshToken(refreshTokenValue, DateTime.UtcNow.AddDays(14));
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();

        var query = new LoginByRefreshTokenQuery(refreshTokenValue);
        
        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("mock_access_token");
        result.Value.RefreshToken.Should().NotBe(refreshTokenValue);
        
        var userWithTokens = await Context.Users
            .AsNoTracking()
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Id == user.Id);
        
        userWithTokens.Should().NotBeNull();
        userWithTokens.RefreshTokens.Count.Should().Be(1);
        userWithTokens.RefreshTokens.First().Should().NotBe(refreshTokenValue);
    }

    [Fact]
    public async Task Handle_ShouldReturnTokenPair_WhenRefreshTokenIsNotValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        user.AddRefreshToken(refreshTokenValue, DateTime.UtcNow.AddDays(14));
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();

        var query = new LoginByRefreshTokenQuery("8391831983");
        
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
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        user.AddRefreshToken(refreshTokenValue, DateTime.UtcNow.AddDays(-1));
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();

        var query = new LoginByRefreshTokenQuery(refreshTokenValue);
        
        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(RefreshTokenErrors.RelevantNotFound);
    }
}