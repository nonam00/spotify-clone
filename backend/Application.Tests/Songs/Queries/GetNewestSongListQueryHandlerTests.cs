using FluentAssertions;

using Application.Songs.Queries.GetNewestSongList;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Songs.Queries;

public class GetNewestSongListQueryHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnNewestPublishedSongs()
    {
        // Arrange
        var song1 = Song.Create("Song 1", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg"));
        var song2 = Song.Create("Song 2", "Author", new FilePath("song2.mp3"), new FilePath("img2.jpg"));
        var song3 = Song.Create("Song 3", "Author", new FilePath("song3.mp3"), new FilePath("img3.jpg"));

        song1.Publish();
        song2.Publish();
        // song3 is not published

        await Context.Songs.AddRangeAsync(song1, song2, song3);
        await Context.SaveChangesAsync();
        
        var query = new GetNewestSongListQuery();

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        result.Value.Should().NotBeNull();
        result.Value.Songs.Should().HaveCount(2);
        result.Value.Songs.Should().OnlyContain(s => s.IsPublished);
    }
}
