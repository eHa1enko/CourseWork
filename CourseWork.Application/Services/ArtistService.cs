using CourseWork.Application.DTOs;
using CourseWork.Application.Interfaces.Repositories;
using CourseWork.Application.Interfaces.Services;

namespace CourseWork.Application.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistService(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<IEnumerable<ArtistDto>> GetAllAsync()
        {
            var artists = await _artistRepository.GetAllAsync();
            return artists.Select(ToDto);
        }

        public async Task<ArtistDto?> GetByIdAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);
            return artist is null ? null : ToDto(artist);
        }

        public async Task<IEnumerable<ArtistDto>> SearchAsync(string query)
        {
            var artists = await _artistRepository.SearchAsync(query);
            return artists.Select(ToDto);
        }

        public async Task<string?> GetImagePathAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);
            return artist?.ImagePath;
        }

        public async Task<ArtistDto> CreateAsync(string name, string? imagePath)
        {
            var artist = new CourseWork.Entities.Entities.Artist
            {
                Name = name,
                ImagePath = imagePath
            };
            var created = await _artistRepository.AddAsync(artist);
            return ToDto(created);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);
            if (artist is null) return false;
            await _artistRepository.DeleteAsync(artist);
            return true;
        }

        private static ArtistDto ToDto(CourseWork.Entities.Entities.Artist artist) => new()
        {
            Id = artist.Id,
            Name = artist.Name,
            ImageUrl = artist.ImagePath is not null ? $"/api/artists/{artist.Id}/image" : null,
            SongCount = artist.Songs?.Count ?? 0
        };
    }
}
