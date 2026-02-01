using FluentAssertions;

using Application.Users.Queries.CheckLike;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Users.Queries;

public class CheckLikeQueryHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenSongIsLiked()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var song = Song.Create(
            "Test Song",
            "Test Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        song.Publish();
        
        user.LikeSong(song.Id);
        
        await Context.Songs.AddAsync(song);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new CheckLikeQuery(user.Id, song.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenSongIsNotLiked()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var song = Song.Create(
            "Test Song",
            "Test Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        song.Publish();
        
        await Context.Users.AddAsync(user);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var query = new CheckLikeQuery(user.Id, song.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }
}
