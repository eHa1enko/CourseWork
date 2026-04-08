using CourseWork.Entities.Entities;

namespace CourseWork.Application.Interfaces.Repositories
{
    public interface ILikedSongRepository
    {
        Task<IEnumerable<LikedSong>> GetByUserIdAsync(int userId);
        Task<LikedSong?> GetAsync(int userId, int songId);
        Task AddAsync(LikedSong likedSong);
        Task RemoveAsync(LikedSong likedSong);
    }
}
