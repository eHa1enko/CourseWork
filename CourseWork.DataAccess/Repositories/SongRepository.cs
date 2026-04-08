using CourseWork.Application.Interfaces.Repositories;
using CourseWork.DataAccess.Data;
using CourseWork.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.DataAccess.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly AppDbContext _context;

        public SongRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Song>> GetAllAsync()
        {
            return await _context.Songs.Include(s => s.Artist).ToListAsync();
        }

        public async Task<Song?> GetByIdAsync(int id)
        {
            return await _context.Songs.Include(s => s.Artist).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Song>> GetByArtistIdAsync(int artistId)
        {
            return await _context.Songs.Include(s => s.Artist).Where(s => s.ArtistId == artistId).ToListAsync();
        }
    }
}
