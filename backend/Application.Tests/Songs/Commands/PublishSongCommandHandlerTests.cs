using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Songs.Commands.PublishSong;
using Application.Songs.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Songs.Commands;

public class PublishSongCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldPublishSong_WhenSongExists()
    {
        // Arrange
        var song = Song.Create(
            "Test Song",
            new FilePath("song.mp3"),
            new FilePath("image.jpg"),
            "Test Author");
        
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new PublishSongCommand(song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var publishedSong = await Context.Songs.FirstOrDefaultAsync(s => s.Id == song.Id);
        
        publishedSong.Should().NotBeNull();
        publishedSong.IsPublished.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongNotFound()
    {
        // Arrange
        var command = new PublishSongCommand(Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new PublishSongCommand(Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Id");
    }
    
    // TODO: published -> publish
}
