using CourseWork.Application.Interfaces.Repositories;
using CourseWork.DataAccess.Data;
using CourseWork.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.DataAccess.Repositories
{
    public class LikedSongRepository : ILikedSongRepository
    {
        private readonly AppDbContext _context;

        public LikedSongRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LikedSong>> GetByUserIdAsync(int userId)
        {
            return await _context.LikedSongs
                .Include(ls => ls.Song)
                    .ThenInclude(s => s.Artist)
                .Where(ls => ls.UserId == userId)
                .ToListAsync();
        }

        public async Task<LikedSong?> GetAsync(int userId, int songId)
        {
            return await _context.LikedSongs
                .FirstOrDefaultAsync(ls => ls.UserId == userId && ls.SongId == songId);
        }

        public async Task AddAsync(LikedSong likedSong)
        {
            await _context.LikedSongs.AddAsync(likedSong);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(LikedSong likedSong)
        {
            _context.LikedSongs.Remove(likedSong);
            await _context.SaveChangesAsync();
        }
    }
}
