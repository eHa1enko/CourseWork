using CourseWork.Application.DTOs;

namespace CourseWork.Application.Interfaces.Services
{
    public interface ISongService
    {
        Task<IEnumerable<SongDto>> GetAllAsync();
        Task<SongDto?> GetByIdAsync(int id);
        Task<IEnumerable<SongDto>> GetByArtistIdAsync(int artistId);
        Task<IEnumerable<SongDto>> SearchAsync(string query);
        Task<string?> GetFilePathAsync(int id);
        Task<string?> GetCoverPathAsync(int id);
        Task<SongDto> CreateAsync(string title, int artistId, string filePath, string? coverPath, int duration);
        Task<bool> DeleteAsync(int id);
    }
}
