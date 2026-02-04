using FluentAssertions;

using Domain.Models;
using Domain.ValueObjects;
using Application.Songs.Queries.GetSongListBySearch;
using Application.Songs.Enums;

namespace Application.Tests.Songs.Queries;

public class GetSongListBySearchQueryHandlerTests : TestBaseWithPostgresFixture
{
    public GetSongListBySearchQueryHandlerTests(PostgreSqlFixture fixture) : base(fixture) { }

    // Simple tests 
    [Fact]
    public async Task Handle_ShouldReturnSuccessWithSongs_WhenSearchMatchesByTitle()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "UniqueTitleSong",
            "Author",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetSongListBySearchQuery("UniqueTitle", SearchCriteria.Title);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Songs.Should().NotBeNull();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "UniqueTitleSong" && s.IsPublished);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithSongs_WhenSearchMatchesByAuthor()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "Song",
            "UniqueAuthorName",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetSongListBySearchQuery("UniqueAuthor", SearchCriteria.Author);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Author == "UniqueAuthorName" && s.IsPublished);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithSongs_WhenSearchMatchesByAny()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "SomeTitle",
            "SomeAuthor",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetSongListBySearchQuery("Some", SearchCriteria.Any);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "SomeTitle" && s.IsPublished);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoSongsMatch()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "OtherTitle",
            "OtherAuthor",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetSongListBySearchQuery("NonExistentXyZ", SearchCriteria.Any);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyPublishedSongs()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var published = user.UploadSong(
            "PublishedSong",
            "Author",
            new FilePath("s1.mp3"),
            new FilePath("i1.jpg")).Value;
        
        user.UploadSong("UnpublishedSong", "Author", new FilePath("s2.mp3"), new FilePath("i2.jpg"));
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(published);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetSongListBySearchQuery("Song", SearchCriteria.Any);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle();
        result.Value.Songs!.Single().Title.Should().Be("PublishedSong");
        result.Value.Songs.Single().IsPublished.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithEmptyList_WhenNoPublishedSongsExist()
    {
        // Arrange
        var query = new GetSongListBySearchQuery("Anything", SearchCriteria.Any);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldBeCaseInsensitive_WhenSearchingByTitle()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "MixedCaseTitle",
            "Author",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetSongListBySearchQuery("MIXEDCASE", SearchCriteria.Title);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "MixedCaseTitle");
    }
    
    [Fact]
    public async Task Handle_ShouldTrimSearchString_WhenLeadingAndTrailingSpaces()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "Trimmed",
            "Author",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetSongListBySearchQuery("  Trimmed  ", SearchCriteria.Title);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "Trimmed");
    }

    // Similarity threshold (trigram) search tests
    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_TypoInTitle()
    {
        // Arrange: similarity threshold is 0.1 for length <= 6, 0.2 for length > 6
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "Beautiful",
            "Artist",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetSongListBySearchQuery("Beautifull", SearchCriteria.Title); // typo: extra 'l'

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "Beautiful");
    }

    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_ShortQueryTitle()
    {
        // Arrange: "Sng" vs "Song" - trigram similarity should exceed 0.1
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "Song",
            "Author",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetSongListBySearchQuery("Sng", SearchCriteria.Title);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "Song");
    }

    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_ByAuthor()
    {
        // Arrange: typo in author name
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "Track",
            "Christopher",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetSongListBySearchQuery("Christofer", SearchCriteria.Author); // typo: 'e' instead of 'h'

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Author == "Christopher");
    }

    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_ByAny()
    {
        // Arrange: typo matches either title or author
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "Melody",
            "Beethoven",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;

        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetSongListBySearchQuery("Beethven", SearchCriteria.Any); // typo: 'v' instead of 'o'

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Author == "Beethoven");
    }

    [Fact]
    public async Task Handle_ShouldOrderExactMatchBeforeSimilarityMatch_WhenMultipleSongsMatch()
    {
        // Arrange: one exact substring match, one trigram-only match - exact should appear first
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var exactMatch = user.UploadSong(
            "RockMusic",
            "Band",
            new FilePath("s1.mp3"),
            new FilePath("i1.jpg")).Value;
        var similarityMatch = user.UploadSong(
            "Rock",
            "Other",
            new FilePath("s2.mp3"),
            new FilePath("i2.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSongs([exactMatch, similarityMatch]);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetSongListBySearchQuery("RockMusic", SearchCriteria.Title);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().HaveCount(2);
        result.Value.Songs!.First().Title.Should().Be("RockMusic");
    }

    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_ThresholdStricterForLongQuery()
    {
        // Arrange: long query (length > 6) uses threshold 0.2 - close typo should still match
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var song = user.UploadSong(
            "Something",
            "Someone",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetSongListBySearchQuery("Somethng", SearchCriteria.Title); // missing 'i'

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "Something");
    }
    
    // Validation tests 
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchStringIsEmpty()
    {
        // Arrange
        var query = new GetSongListBySearchQuery("", SearchCriteria.Title);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Search string");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchStringIsWhitespaceOnly()
    {
        // Arrange
        var query = new GetSongListBySearchQuery("   ", SearchCriteria.Any);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Search string");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchStringIsTooShort()
    {
        // Arrange
        var query = new GetSongListBySearchQuery("ab", SearchCriteria.Title);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("at least 3 characters");
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenSearchStringIsExactlyThreeCharacters()
    {
        // Arrange
        var query = new GetSongListBySearchQuery("abc", SearchCriteria.Any);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchStringExceedsMaxLength()
    {
        // Arrange
        var longSearch = new string('a', 101);
        var query = new GetSongListBySearchQuery(longSearch, SearchCriteria.Title);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("100");
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenSearchStringIsExactlyMaxLength()
    {
        // Arrange
        var maxLengthSearch = new string('a', 100);
        var query = new GetSongListBySearchQuery(maxLengthSearch, SearchCriteria.Any);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}