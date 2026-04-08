using CourseWork.Application.DTOs;

namespace CourseWork.Application.Interfaces.Services
{
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> GetAllAsync();
        Task<ArtistDto?> GetByIdAsync(int id);
        Task<string?> GetImagePathAsync(int id);
    }
}
