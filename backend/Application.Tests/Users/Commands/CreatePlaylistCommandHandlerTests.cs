using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Users.Commands.CreatePlaylist;
using Application.Users.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Commands;

public class CreatePlaylistCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldCreatePlaylist_WhenUserExists()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        var command = new CreatePlaylistCommand(user.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var playlist = await Context.Playlists.SingleOrDefaultAsync(p => p.Id == result.Value);
        
        playlist.Should().NotBeNull();
        playlist.Title.Should().Be("Playlist #1");
        playlist.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new CreatePlaylistCommand(Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldCreateMultiplePlaylists_WithCorrectTitles()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        var command1 = new CreatePlaylistCommand(user.Id);
        var command2 = new CreatePlaylistCommand(user.Id);

        // Act
        var result1 = await Mediator.Send(command1, CancellationToken.None);
        var result2 = await Mediator.Send(command2, CancellationToken.None);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        
        var playlist1 = await Context.Playlists.FirstOrDefaultAsync(p => p.Id == result1.Value);
        var playlist2 = await Context.Playlists.FirstOrDefaultAsync(p => p.Id == result2.Value);
        
        playlist1!.Title.Should().Be("Playlist #1");
        playlist2!.Title.Should().Be("Playlist #2");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new CreatePlaylistCommand(Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }
}
