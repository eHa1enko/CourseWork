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

        public async Task<IEnumerable<SongDto>> SearchAsync(string query)
        {
            var songs = await _songRepository.SearchAsync(query);
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

        public async Task<SongDto> CreateAsync(string title, int artistId, string filePath, string? coverPath, int duration)
        {
            var song = new CourseWork.Entities.Entities.Song
            {
                Title = title,
                ArtistId = artistId,
                FilePath = filePath,
                CoverPath = coverPath,
                Duration = duration
            };
            var created = await _songRepository.AddAsync(song);
            var full = await _songRepository.GetByIdAsync(created.Id);
            return ToDto(full!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            if (song is null) return false;
            await _songRepository.DeleteAsync(song);
            return true;
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
