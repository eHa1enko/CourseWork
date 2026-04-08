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

        public async Task<string?> GetImagePathAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);
            return artist?.ImagePath;
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
