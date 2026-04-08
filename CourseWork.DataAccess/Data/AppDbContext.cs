using CourseWork.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Song> Songs => Set<Song>();
        public DbSet<LikedSong> LikedSongs => Set<LikedSong>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LikedSong>()
                .HasIndex(ls => new { ls.UserId, ls.SongId })
                .IsUnique();

            modelBuilder.Entity<Song>()
                .HasOne(s => s.Artist)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikedSong>()
                .HasOne(ls => ls.User)
                .WithMany(u => u.LikedSongs)
                .HasForeignKey(ls => ls.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikedSong>()
                .HasOne(ls => ls.Song)
                .WithMany(s => s.LikedBy)
                .HasForeignKey(ls => ls.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
