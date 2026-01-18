using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Users.Commands.LikeSong;
using Application.Users.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Commands;

public class LikeSongCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldLikeSong_WhenUserAndSongExist()
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
            "Test Author",
            user.Id);
        song.Publish();
        
        await Context.Users.AddAsync(user);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new LikeSongCommand(user.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var likedSong = await Context.Set<LikedSong>()
            .AsNoTracking()
            .SingleOrDefaultAsync(ls => ls.UserId == user.Id && ls.SongId == song.Id);
        
        likedSong.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new LikeSongCommand(Guid.NewGuid(), song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongAlreadyLiked()
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
            "Test Author",
            user.Id);
        song.Publish();
        
        user.LikeSong(song.Id);
        
        await Context.Users.AddAsync(user);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new LikeSongCommand(user.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserLikeErrors.AlreadyLiked);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        song.Publish();
        
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new LikeSongCommand(Guid.Empty, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
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
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new LikeSongCommand(user.Id, Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongId");
    }
}
