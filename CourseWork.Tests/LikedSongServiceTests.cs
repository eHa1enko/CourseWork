using CourseWork.Application.Interfaces.Repositories;
using CourseWork.Application.Services;
using CourseWork.Entities.Entities;
using Moq;

namespace CourseWork.Tests;

public class LikedSongServiceTests
{
    private readonly Mock<ILikedSongRepository> _likedSongRepo = new();
    private readonly LikedSongService _sut;

    public LikedSongServiceTests()
    {
        _sut = new LikedSongService(_likedSongRepo.Object);
    }

    [Fact]
    public async Task LikeAsync_WhenNotLikedYet_AddsLike()
    {
        _likedSongRepo.Setup(r => r.GetAsync(1, 10)).ReturnsAsync((LikedSong?)null);
        _likedSongRepo.Setup(r => r.AddAsync(It.IsAny<LikedSong>())).Returns(Task.CompletedTask);

        await _sut.LikeAsync(1, 10);

        _likedSongRepo.Verify(r => r.AddAsync(It.Is<LikedSong>(ls => ls.UserId == 1 && ls.SongId == 10)), Times.Once);
    }

    [Fact]
    public async Task LikeAsync_WhenAlreadyLiked_DoesNotAddDuplicate()
    {
        var existing = new LikedSong { UserId = 1, SongId = 10 };
        _likedSongRepo.Setup(r => r.GetAsync(1, 10)).ReturnsAsync(existing);

        await _sut.LikeAsync(1, 10);

        _likedSongRepo.Verify(r => r.AddAsync(It.IsAny<LikedSong>()), Times.Never);
    }

    [Fact]
    public async Task UnlikeAsync_WhenLikeExists_RemovesIt()
    {
        var existing = new LikedSong { UserId = 1, SongId = 10 };
        _likedSongRepo.Setup(r => r.GetAsync(1, 10)).ReturnsAsync(existing);
        _likedSongRepo.Setup(r => r.RemoveAsync(existing)).Returns(Task.CompletedTask);

        await _sut.UnlikeAsync(1, 10);

        _likedSongRepo.Verify(r => r.RemoveAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UnlikeAsync_WhenLikeDoesNotExist_DoesNothing()
    {
        _likedSongRepo.Setup(r => r.GetAsync(1, 10)).ReturnsAsync((LikedSong?)null);

        await _sut.UnlikeAsync(1, 10);

        _likedSongRepo.Verify(r => r.RemoveAsync(It.IsAny<LikedSong>()), Times.Never);
    }

    [Fact]
    public async Task IsLikedAsync_WhenLikeExists_ReturnsTrue()
    {
        _likedSongRepo.Setup(r => r.GetAsync(1, 10)).ReturnsAsync(new LikedSong());

        var result = await _sut.IsLikedAsync(1, 10);

        Assert.True(result);
    }

    [Fact]
    public async Task IsLikedAsync_WhenLikeDoesNotExist_ReturnsFalse()
    {
        _likedSongRepo.Setup(r => r.GetAsync(1, 10)).ReturnsAsync((LikedSong?)null);

        var result = await _sut.IsLikedAsync(1, 10);

        Assert.False(result);
    }

    [Fact]
    public async Task GetLikedSongsAsync_ReturnsMappedDtos()
    {
        var likedSongs = new List<LikedSong>
        {
            new()
            {
                UserId = 1,
                SongId = 10,
                Song = new Song
                {
                    Id = 10,
                    Title = "Test Song",
                    Duration = 200,
                    FilePath = "http://audio.url",
                    CoverPath = "http://cover.url",
                    ArtistId = 5,
                    Artist = new Artist { Id = 5, Name = "Test Artist" }
                }
            }
        };
        _likedSongRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(likedSongs);

        var result = (await _sut.GetLikedSongsAsync(1)).ToList();

        Assert.Single(result);
        Assert.Equal(10, result[0].Id);
        Assert.Equal("Test Song", result[0].Title);
        Assert.Equal("Test Artist", result[0].ArtistName);
        Assert.True(result[0].IsLiked);
    }

    [Fact]
    public async Task GetLikedSongsAsync_WhenNoLikes_ReturnsEmptyCollection()
    {
        _likedSongRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<LikedSong>());

        var result = await _sut.GetLikedSongsAsync(1);

        Assert.Empty(result);
    }
}
