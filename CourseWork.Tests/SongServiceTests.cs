using CourseWork.Application.Interfaces.Repositories;
using CourseWork.Application.Services;
using CourseWork.Entities.Entities;
using Moq;

namespace CourseWork.Tests;

public class SongServiceTests
{
    private readonly Mock<ISongRepository> _songRepo = new();
    private readonly SongService _sut;

    public SongServiceTests()
    {
        _sut = new SongService(_songRepo.Object);
    }

    private static Song MakeSong(int id = 1, string title = "Song", string artistName = "Artist") => new()
    {
        Id = id,
        Title = title,
        Duration = 180,
        FilePath = "http://file.url",
        CoverPath = "http://cover.url",
        ArtistId = 1,
        Artist = new Artist { Id = 1, Name = artistName }
    };

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        _songRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Song> { MakeSong(1, "A"), MakeSong(2, "B") });

        var result = (await _sut.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Title);
        Assert.Equal("B", result[1].Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFound_ReturnsMappedDto()
    {
        _songRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeSong(1, "MySong"));

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("MySong", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _songRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Song?)null);

        var result = await _sut.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByArtistIdAsync_ReturnsSongsForArtist()
    {
        _songRepo.Setup(r => r.GetByArtistIdAsync(5)).ReturnsAsync(new List<Song> { MakeSong(1), MakeSong(2) });

        var result = await _sut.GetByArtistIdAsync(5);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsSongsMatchingQuery()
    {
        _songRepo.Setup(r => r.SearchAsync("rock")).ReturnsAsync(new List<Song> { MakeSong(1, "Rock Song") });

        var result = (await _sut.SearchAsync("rock")).ToList();

        Assert.Single(result);
        Assert.Equal("Rock Song", result[0].Title);
    }

    [Fact]
    public async Task GetFilePathAsync_WhenSongExists_ReturnsFilePath()
    {
        _songRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeSong(1));

        var result = await _sut.GetFilePathAsync(1);

        Assert.Equal("http://file.url", result);
    }

    [Fact]
    public async Task GetFilePathAsync_WhenSongNotFound_ReturnsNull()
    {
        _songRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Song?)null);

        var result = await _sut.GetFilePathAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCoverPathAsync_WhenSongExists_ReturnsCoverPath()
    {
        _songRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeSong(1));

        var result = await _sut.GetCoverPathAsync(1);

        Assert.Equal("http://cover.url", result);
    }

    [Fact]
    public async Task CreateAsync_SavesSongAndReturnsDtoWithArtist()
    {
        var created = MakeSong(42, "New Song", "New Artist");
        _songRepo.Setup(r => r.AddAsync(It.IsAny<Song>())).ReturnsAsync(created);
        _songRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(created);

        var result = await _sut.CreateAsync("New Song", 1, "http://file.url", "http://cover.url", 180);

        Assert.Equal(42, result.Id);
        Assert.Equal("New Song", result.Title);
        Assert.Equal("New Artist", result.ArtistName);
        _songRepo.Verify(r => r.AddAsync(It.IsAny<Song>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenSongExists_DeletesAndReturnsTrue()
    {
        var song = MakeSong(1);
        _songRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(song);
        _songRepo.Setup(r => r.DeleteAsync(song)).Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(1);

        Assert.True(result);
        _songRepo.Verify(r => r.DeleteAsync(song), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenSongNotFound_ReturnsFalse()
    {
        _songRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Song?)null);

        var result = await _sut.DeleteAsync(99);

        Assert.False(result);
        _songRepo.Verify(r => r.DeleteAsync(It.IsAny<Song>()), Times.Never);
    }
}
