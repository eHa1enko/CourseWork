using CourseWork.Entities.Entities;

namespace CourseWork.Application.Interfaces.Repositories
{
    public interface ISongRepository
    {
        Task<IEnumerable<Song>> GetAllAsync();
        Task<Song?> GetByIdAsync(int id);
        Task<IEnumerable<Song>> GetByArtistIdAsync(int artistId);
    }
}
