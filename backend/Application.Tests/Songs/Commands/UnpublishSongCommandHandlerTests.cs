using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Application.Songs.Commands.UnpublishSong;
using Application.Songs.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Songs.Commands;

public class UnpublishSongCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUnpublishSong_WhenSongExists()
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
        
        Context.ChangeTracker.Clear();
        
        var command = new UnpublishSongCommand(song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var unpublishedSong = await Context.Songs.FirstOrDefaultAsync(s => s.Id == song.Id);
        unpublishedSong.Should().NotBeNull();
        unpublishedSong.IsPublished.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongNotFound()
    {
        // Arrange
        var command = new UnpublishSongCommand(Guid.NewGuid());

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
        var command = new UnpublishSongCommand(Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Id");
    }
    
    // TODO: unpublished -> unpublish 
}
