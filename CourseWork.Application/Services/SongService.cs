using CourseWork.Application.DTOs;
using CourseWork.Application.Interfaces.Repositories;
using CourseWork.Application.Interfaces.Services;

namespace CourseWork.Application.Services
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;

        public SongService(ISongRepository songRepository)
        {
            _songRepository = songRepository;
        }

        public async Task<IEnumerable<SongDto>> GetAllAsync()
        {
            var songs = await _songRepository.GetAllAsync();
            return songs.Select(ToDto);
        }

        public async Task<SongDto?> GetByIdAsync(int id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            return song is null ? null : ToDto(song);
        }

        public async Task<IEnumerable<SongDto>> GetByArtistIdAsync(int artistId)
        {
            var songs = await _songRepository.GetByArtistIdAsync(artistId);
            return songs.Select(ToDto);
        }

        public async Task<string?> GetFilePathAsync(int id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            return song?.FilePath;
        }

        public async Task<string?> GetCoverPathAsync(int id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            return song?.CoverPath;
        }

        private static SongDto ToDto(CourseWork.Entities.Entities.Song song) => new()
        {
            Id = song.Id,
            Title = song.Title,
            Duration = song.Duration,
            CoverUrl = song.CoverPath is not null ? $"/api/songs/{song.Id}/cover" : null,
            ArtistId = song.ArtistId,
            ArtistName = song.Artist?.Name ?? string.Empty
        };
    }
}
