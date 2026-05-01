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

        public async Task<IEnumerable<Song>> SearchAsync(string query)
        {
            return await _context.Songs
                .Include(s => s.Artist)
                .Where(s => s.Title.ToLower().Contains(query.ToLower()) ||
                            s.Artist.Name.ToLower().Contains(query.ToLower()))
                .ToListAsync();
        }

        public async Task<Song> AddAsync(Song song)
        {
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
            return song;
        }

        public async Task DeleteAsync(Song song)
        {
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
        }
    }
}
