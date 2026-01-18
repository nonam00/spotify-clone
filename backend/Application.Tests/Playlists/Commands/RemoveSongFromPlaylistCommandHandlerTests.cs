using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Playlists.Commands.RemoveSongFromPlaylist;
using Application.Playlists.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Commands;

public class RemoveSongFromPlaylistCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldRemoveSongFromPlaylist_WhenValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist");
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        playlist.AddSong(song.Id);
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        var command = new RemoveSongFromPlaylistCommand(user.Id, playlist.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var playlistSong = await Context.Set<PlaylistSong>()
            .FirstOrDefaultAsync(ps => ps.PlaylistId == playlist.Id && ps.SongId == song.Id);
        
        playlistSong.Should().BeNull();
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
        
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        await Context.Users.AddAsync(user);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new RemoveSongFromPlaylistCommand(user.Id, Guid.NewGuid(), song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongNotInPlaylist()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist");
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new RemoveSongFromPlaylistCommand(user.Id, playlist.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.SongNotInPlaylist);
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
        
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        playlist.AddSong(song.Id);
        
        await Context.Users.AddRangeAsync(owner, otherUser);
        await Context.Playlists.AddAsync(playlist);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new RemoveSongFromPlaylistCommand(otherUser.Id, playlist.Id, song.Id);

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
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist");
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new RemoveSongFromPlaylistCommand(Guid.Empty, playlist.Id, song.Id);

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
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        await Context.Users.AddAsync(user);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new RemoveSongFromPlaylistCommand(user.Id, Guid.Empty, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("PlaylistId");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongIdIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist");
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();
        
        var command = new RemoveSongFromPlaylistCommand(user.Id, playlist.Id, Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongId");
    }
}
