using FluentAssertions;

using Domain.Models;
using Domain.ValueObjects;
using Application.Songs.Queries.GetSongById;
using Application.Songs.Errors;

namespace Application.Tests.Songs.Queries;

public class GetSongByIdQueryHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_ShouldReturnSong_WhenSongExistsAndPublished()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var song = user.UploadSong("Song", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg")).Value;

        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetSongByIdQuery(song.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        result.Value.Should().NotBeNull();
        result.Value.IsPublished.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSong_WhenSongExistsButNotPublished()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var song = user.UploadSong("Song", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg")).Value;

        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var query = new GetSongByIdQuery(song.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        result.Value.Should().NotBeNull();
        result.Value.IsPublished.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongNotFound()
    {
        // Arrange
        var query = new GetSongByIdQuery(Guid.NewGuid());

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongIdIsEmpty()
    {
        // Arrange
        var query = new GetSongByIdQuery(Guid.Empty);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongId");
    }
}
