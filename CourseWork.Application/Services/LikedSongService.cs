using CourseWork.Application.DTOs;
using CourseWork.Application.Interfaces.Repositories;
using CourseWork.Application.Interfaces.Services;
using CourseWork.Entities.Entities;

namespace CourseWork.Application.Services
{
    public class LikedSongService : ILikedSongService
    {
        private readonly ILikedSongRepository _likedSongRepository;

        public LikedSongService(ILikedSongRepository likedSongRepository)
        {
            _likedSongRepository = likedSongRepository;
        }

        public async Task<IEnumerable<SongDto>> GetLikedSongsAsync(int userId)
        {
            var liked = await _likedSongRepository.GetByUserIdAsync(userId);
            return liked.Select(ls => new SongDto
            {
                Id = ls.Song.Id,
                Title = ls.Song.Title,
                Duration = ls.Song.Duration,
                CoverUrl = ls.Song.CoverPath is not null ? $"/api/songs/{ls.Song.Id}/cover" : null,
                ArtistId = ls.Song.ArtistId,
                ArtistName = ls.Song.Artist?.Name ?? string.Empty,
                IsLiked = true
            });
        }

        public async Task LikeAsync(int userId, int songId)
        {
            var existing = await _likedSongRepository.GetAsync(userId, songId);
            if (existing is not null) return;

            await _likedSongRepository.AddAsync(new LikedSong
            {
                UserId = userId,
                SongId = songId
            });
        }

        public async Task UnlikeAsync(int userId, int songId)
        {
            var liked = await _likedSongRepository.GetAsync(userId, songId);
            if (liked is null) return;

            await _likedSongRepository.RemoveAsync(liked);
        }

        public async Task<bool> IsLikedAsync(int userId, int songId)
        {
            var liked = await _likedSongRepository.GetAsync(userId, songId);
            return liked is not null;
        }
    }
}
