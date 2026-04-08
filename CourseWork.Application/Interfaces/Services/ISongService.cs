using CourseWork.Application.DTOs;

namespace CourseWork.Application.Interfaces.Services
{
    public interface ISongService
    {
        Task<IEnumerable<SongDto>> GetAllAsync();
        Task<SongDto?> GetByIdAsync(int id);
        Task<IEnumerable<SongDto>> GetByArtistIdAsync(int artistId);
        Task<string?> GetFilePathAsync(int id);
        Task<string?> GetCoverPathAsync(int id);
    }
}
