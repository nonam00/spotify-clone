using FluentAssertions; 
using Microsoft.EntityFrameworkCore;

using Application.Playlists.Commands.UpdatePlaylist;
using Application.Playlists.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Commands;

public class UpdatePlaylistCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUpdatePlaylist_WhenValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "Old Title", "Old Description");
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();

        var command = new UpdatePlaylistCommand(
            UserId: user.Id,
            PlaylistId: playlist.Id,
            "New Title",
            "New Description", 
            "new_image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedPlaylist = await Context.Playlists.FirstOrDefaultAsync(p => p.Id == playlist.Id);
        updatedPlaylist.Should().NotBeNull();
        updatedPlaylist!.Title.Should().Be("New Title");
        updatedPlaylist.Description.Should().Be("New Description");
        updatedPlaylist.ImagePath.Value.Should().Be("new_image.jpg");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlaylistNotFound()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
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
        var owner = User.Create(
            new Email("owner@example.com"),
            new PasswordHash("hashed_password"),
            "Owner");
        owner.Activate();
        
        var otherUser = User.Create(
            new Email("other@example.com"),
            new PasswordHash("hashed_password"),
            "Other User");
        otherUser.Activate();
        
        var playlist = Playlist.Create(owner.Id, "My Playlist");
        
        await Context.Users.AddAsync(owner);
        await Context.Users.AddAsync(otherUser);
        await Context.Playlists.AddAsync(playlist);
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
    public async Task Handle_ShouldSetDescriptionToNull_WhenNewDescriptionIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "Old Title", "Old Description");
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new UpdatePlaylistCommand(
            user.Id,
            playlist.Id,
            "New Title",
            "",
            "new_image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedPlaylist = await Context.Playlists.FirstOrDefaultAsync(p => p.Id == playlist.Id);
        updatedPlaylist.Should().NotBeNull();
        updatedPlaylist!.Title.Should().Be("New Title");
        updatedPlaylist.Description.Should().Be(null);
        updatedPlaylist.ImagePath.Value.Should().Be("new_image.jpg");
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateImagePath_WhenNewImagePathIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(
            user.Id,
            title: "Old Title",
            description: null,
            new FilePath("old_image.jpg"));
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
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
        updatedPlaylist!.Title.Should().Be("New Title");
        updatedPlaylist.Description.Should().Be(null);
        updatedPlaylist.ImagePath.Value.Should().Be("old_image.jpg");
    }
    
    [Fact]
    public async Task Handle_ShouldNotUpdateImagePath_WhenNewImagePathIsNull()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(
            user.Id,
            title: "Old Title",
            description: null,
            new FilePath("old_image.jpg"));
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
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
        updatedPlaylist!.Title.Should().Be("New Title");
        updatedPlaylist.Description.Should().Be(null);
        updatedPlaylist.ImagePath.Value.Should().Be("old_image.jpg");
    }
}
