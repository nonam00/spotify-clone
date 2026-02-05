using FluentAssertions; 
using Microsoft.EntityFrameworkCore;

using Application.Playlists.Commands.UpdatePlaylist;
using Application.Playlists.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Commands;

public class UpdatePlaylistCommandHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_ShouldUpdatePlaylist_WhenValid()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new UpdatePlaylistCommand(
            UserId: user.Id,
            PlaylistId: playlist.Id,
            "New Title",
            "New Description",
            "");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedPlaylist = await Context.Playlists.FirstOrDefaultAsync(p => p.Id == playlist.Id);
        updatedPlaylist.Should().NotBeNull();
        updatedPlaylist.Title.Should().Be("New Title");
        updatedPlaylist.Description.Should().Be("New Description");
        updatedPlaylist.ImagePath.Value.Should().Be("");
    }
    
    [Fact]
    public async Task Handle_ShouldSetDescriptionToNull_WhenNewDescriptionIsEmpty()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;
        playlist.UpdateDetails("Old Title", "Old Description", new FilePath(""));
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdatePlaylistCommand(
            user.Id,
            playlist.Id,
            "New Title",
            "",
            "");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedPlaylist = await Context.Playlists.FirstOrDefaultAsync(p => p.Id == playlist.Id);
        updatedPlaylist.Should().NotBeNull();
        updatedPlaylist.Title.Should().Be("New Title");
        updatedPlaylist.Description.Should().Be(null);
        updatedPlaylist.ImagePath.Value.Should().Be("");
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateImagePath_WhenNewImagePathIsEmpty()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        playlist.UpdateDetails("Old title", null, new FilePath("old_image.jpg"));
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdatePlaylistCommand(
            user.Id,
            playlist.Id,
            "New Title",
            null,
            "");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedPlaylist = await Context.Playlists.FirstOrDefaultAsync(p => p.Id == playlist.Id);
        updatedPlaylist.Should().NotBeNull();
        updatedPlaylist.Title.Should().Be("New Title");
        updatedPlaylist.Description.Should().Be(null);
        updatedPlaylist.ImagePath.Value.Should().Be("old_image.jpg");
    }
    
    [Fact]
    public async Task Handle_ShouldNotUpdateImagePath_WhenNewImagePathIsNull()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        playlist.UpdateDetails("Old Title", null, new FilePath("old_image.jpg"));
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdatePlaylistCommand(
            user.Id,
            playlist.Id,
            "New Title",
            null,
            null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedPlaylist = await Context.Playlists.FirstOrDefaultAsync(p => p.Id == playlist.Id);
        updatedPlaylist.Should().NotBeNull();
        updatedPlaylist.Title.Should().Be("New Title");
        updatedPlaylist.Description.Should().Be(null);
        updatedPlaylist.ImagePath.Value.Should().Be("old_image.jpg");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlaylistNotFound()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new UpdatePlaylistCommand(
            user.Id,
            Guid.NewGuid(),
            "New Title",
            null,
            null);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotOwner()
    {
        // Arrange
        var owner = User.Create(new Email("owner@example.com"), new PasswordHash("hashed_password"), "Owner");
        owner.Activate();

        var otherUser = User.Create(
            new Email("other@example.com"),
            new PasswordHash("hashed_password"),
            "Other User");
        otherUser.Activate();

        var playlist = owner.CreatePlaylist().Value;

        await Context.Users.AddRangeAsync(owner, otherUser);
        await Context.SaveChangesAsync();

        var command = new UpdatePlaylistCommand(
            otherUser.Id,
            playlist.Id,
            "New Title",
            null,
            null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.OwnershipError);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new UpdatePlaylistCommand(
            Guid.Empty,
            Guid.NewGuid(),
            "New Title",
            null,
            null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPlaylistIdIsEmpty()
    {
        // Arrange
        var command = new UpdatePlaylistCommand(
            Guid.NewGuid(),
            Guid.Empty,
            "New Title",
            null,
            null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("PlaylistId");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenTitleIsEmpty()
    {
        // Arrange
        var command = new UpdatePlaylistCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            null,
            null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Title");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenTitleIsWhiteSpaces()
    {
        // Arrange
        var command = new UpdatePlaylistCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "   ",
            null,
            null);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Title");
    }
}
