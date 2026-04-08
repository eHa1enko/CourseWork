using CourseWork.Application.DTOs;

namespace CourseWork.Application.Interfaces.Services
{
    public interface ILikedSongService
    {
        Task<IEnumerable<SongDto>> GetLikedSongsAsync(int userId);
        Task LikeAsync(int userId, int songId);
        Task UnlikeAsync(int userId, int songId);
        Task<bool> IsLikedAsync(int userId, int songId);
    }
}
