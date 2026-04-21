using CourseWork.Application.DTOs;

namespace CourseWork.Application.Interfaces.Services
{
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> GetAllAsync();
        Task<ArtistDto?> GetByIdAsync(int id);
        Task<IEnumerable<ArtistDto>> SearchAsync(string query);
        Task<string?> GetImagePathAsync(int id);
        Task<ArtistDto> CreateAsync(string name, string? imagePath);
        Task<bool> DeleteAsync(int id);
    }
}
