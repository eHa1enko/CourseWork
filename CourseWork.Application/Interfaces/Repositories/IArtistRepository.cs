using CourseWork.Entities.Entities;

namespace CourseWork.Application.Interfaces.Repositories
{
    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetAllAsync();
        Task<Artist?> GetByIdAsync(int id);
    }
}
