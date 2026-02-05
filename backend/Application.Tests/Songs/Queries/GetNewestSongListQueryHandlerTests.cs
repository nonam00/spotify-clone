using FluentAssertions;

using Domain.Models;
using Domain.ValueObjects;
using Application.Songs.Queries.GetNewestSongList;

namespace Application.Tests.Songs.Queries;

public class GetNewestSongListQueryHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_ShouldReturnNewestPublishedSongs()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var song1 = user.UploadSong("Song 1", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg")).Value;
        var song2 = user.UploadSong("Song 2", "Author", new FilePath("song2.mp3"), new FilePath("img2.jpg")).Value;
        var song3 = user.UploadSong("Song 3", "Author", new FilePath("song3.mp3"), new FilePath("img3.jpg")).Value;

        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSongs([song1, song2]); // song3 is not published

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
