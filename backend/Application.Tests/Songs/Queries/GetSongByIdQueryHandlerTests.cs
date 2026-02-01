using FluentAssertions;

using Application.Songs.Queries.GetSongById;
using Application.Songs.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Songs.Queries;

public class GetSongByIdQueryHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnSong_WhenSongExistsAndPublished()
    {
        // Arrange
        var song = Song.Create(
            "Test Song",
            "Test Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        song.Publish();
        
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var query = new GetSongByIdQuery(song.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("Test Song");
        result.Value.Author.Should().Be("Test Author");
        result.Value.IsPublished.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSong_WhenSongExistsButNotPublished()
    {
        // Arrange
        var song = Song.Create(
            "Test Song",
            "Test Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var query = new GetSongByIdQuery(song.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("Test Song");
        result.Value.Author.Should().Be("Test Author");
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
