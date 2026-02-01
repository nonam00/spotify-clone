using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Playlists.Commands.AddSongToPlaylist;
using Application.Playlists.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Commands;

public class AddSongToPlaylistCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldAddSongToPlaylist_WhenValid()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        
        var song = Song.Create("Song", "Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        
        song.Publish();
        
        await Context.Songs.AddAsync(song);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new AddSongToPlaylistCommand(user.Id, playlist.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var playlistSong = await Context.Set<PlaylistSong>()
            .FirstOrDefaultAsync(ps => ps.PlaylistId == playlist.Id && ps.SongId == song.Id);
        
        playlistSong.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlaylistNotFound()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var song = Song.Create("Song", "Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        song.Publish();
        
        await Context.Songs.AddAsync(song);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new AddSongToPlaylistCommand(user.Id, Guid.NewGuid(), song.Id);

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
        
        var song = Song.Create("TSong", "Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        song.Publish();
        
        await Context.Songs.AddAsync(song);
        await Context.Users.AddRangeAsync(owner, otherUser);
        await Context.SaveChangesAsync();
        
        var command = new AddSongToPlaylistCommand(otherUser.Id, playlist.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.OwnershipError);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongAlreadyInPlaylist()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;
        
        var song = Song.Create("Song", "Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        song.Publish();
        
        playlist.AddSong(song);
        
        await Context.Songs.AddAsync(song);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new AddSongToPlaylistCommand(user.Id, playlist.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistDomainErrors.AlreadyContainsSong);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAddingUnpublishedSong()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        
        var song = Song.Create(
            "Test Song",
            "Test Author",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"));

        await Context.Songs.AddAsync(song);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new AddSongToPlaylistCommand(user.Id, playlist.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistDomainErrors.UnpublishedSong);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new AddSongToPlaylistCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

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
        var command = new AddSongToPlaylistCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

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
        var command = new AddSongToPlaylistCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongId");
    }
}
