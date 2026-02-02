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
        
        var song = user.UploadSong("Song", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);        
        
        user.LikeSong(song.Id);
        
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
        
        var song = user.UploadSong("Song", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);        
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new CheckLikeQuery(user.Id, song.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }
}
