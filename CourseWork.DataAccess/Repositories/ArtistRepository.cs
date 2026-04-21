using CourseWork.Application.Interfaces.Repositories;
using CourseWork.DataAccess.Data;
using CourseWork.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.DataAccess.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly AppDbContext _context;

        public ArtistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Artist>> GetAllAsync()
        {
            return await _context.Artists.Include(a => a.Songs).ToListAsync();
        }

        public async Task<Artist?> GetByIdAsync(int id)
        {
            return await _context.Artists.Include(a => a.Songs).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Artist>> SearchAsync(string query)
        {
            return await _context.Artists
                .Include(a => a.Songs)
                .Where(a => a.Name.ToLower().Contains(query.ToLower()))
                .ToListAsync();
        }

        public async Task<Artist> AddAsync(Artist artist)
        {
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            return artist;
        }

        public async Task DeleteAsync(Artist artist)
        {
            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
        }
    }
}
