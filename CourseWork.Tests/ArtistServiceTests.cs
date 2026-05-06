using CourseWork.Application.Interfaces.Repositories;
using CourseWork.Application.Services;
using CourseWork.Entities.Entities;
using Moq;

namespace CourseWork.Tests;

public class ArtistServiceTests
{
    private readonly Mock<IArtistRepository> _artistRepo = new();
    private readonly ArtistService _sut;

    public ArtistServiceTests()
    {
        _sut = new ArtistService(_artistRepo.Object);
    }

    private static Artist MakeArtist(int id = 1, string name = "Artist", string? imagePath = null) => new()
    {
        Id = id,
        Name = name,
        ImagePath = imagePath,
        Songs = new List<Song>()
    };

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        _artistRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Artist>
        {
            MakeArtist(1, "Artist A"),
            MakeArtist(2, "Artist B")
        });

        var result = (await _sut.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Artist A", result[0].Name);
        Assert.Equal("Artist B", result[1].Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFound_ReturnsMappedDto()
    {
        _artistRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeArtist(1, "Eminem"));

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Eminem", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _artistRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Artist?)null);

        var result = await _sut.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_SongCountReflectsArtistSongs()
    {
        var artist = new Artist
        {
            Id = 1,
            Name = "Artist",
            Songs = new List<Song> { new(), new(), new() }
        };
        _artistRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Artist> { artist });

        var result = (await _sut.GetAllAsync()).ToList();

        Assert.Equal(3, result[0].SongCount);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingArtists()
    {
        _artistRepo.Setup(r => r.SearchAsync("emi")).ReturnsAsync(new List<Artist> { MakeArtist(1, "Eminem") });

        var result = (await _sut.SearchAsync("emi")).ToList();

        Assert.Single(result);
        Assert.Equal("Eminem", result[0].Name);
    }

    [Fact]
    public async Task GetImagePathAsync_WhenArtistExists_ReturnsImagePath()
    {
        _artistRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeArtist(1, "Artist", "http://img.url"));

        var result = await _sut.GetImagePathAsync(1);

        Assert.Equal("http://img.url", result);
    }

    [Fact]
    public async Task GetImagePathAsync_WhenArtistNotFound_ReturnsNull()
    {
        _artistRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Artist?)null);

        var result = await _sut.GetImagePathAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_SavesArtistAndReturnsDto()
    {
        var created = MakeArtist(10, "New Artist", "http://img.url");
        _artistRepo.Setup(r => r.AddAsync(It.IsAny<Artist>())).ReturnsAsync(created);

        var result = await _sut.CreateAsync("New Artist", "http://img.url");

        Assert.Equal(10, result.Id);
        Assert.Equal("New Artist", result.Name);
        Assert.Equal("http://img.url", result.ImageUrl);
        _artistRepo.Verify(r => r.AddAsync(It.IsAny<Artist>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullImage_ReturnsDtoWithNullImageUrl()
    {
        var created = MakeArtist(11, "No Image Artist", null);
        _artistRepo.Setup(r => r.AddAsync(It.IsAny<Artist>())).ReturnsAsync(created);

        var result = await _sut.CreateAsync("No Image Artist", null);

        Assert.Null(result.ImageUrl);
    }

    [Fact]
    public async Task DeleteAsync_WhenArtistExists_DeletesAndReturnsTrue()
    {
        var artist = MakeArtist(1);
        _artistRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(artist);
        _artistRepo.Setup(r => r.DeleteAsync(artist)).Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(1);

        Assert.True(result);
        _artistRepo.Verify(r => r.DeleteAsync(artist), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenArtistNotFound_ReturnsFalse()
    {
        _artistRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Artist?)null);

        var result = await _sut.DeleteAsync(99);

        Assert.False(result);
        _artistRepo.Verify(r => r.DeleteAsync(It.IsAny<Artist>()), Times.Never);
    }
}
