using CourseWork.Entities.Entities;

namespace CourseWork.Application.Interfaces.Repositories
{
    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetAllAsync();
        Task<Artist?> GetByIdAsync(int id);
        Task<IEnumerable<Artist>> SearchAsync(string query);
        Task<Artist> AddAsync(Artist artist);
        Task DeleteAsync(Artist artist);
    }
}
