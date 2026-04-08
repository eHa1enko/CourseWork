using CourseWork.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.DataAccess.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context, string webRootPath)
        {
            context.Database.Migrate();

            if (context.Artists.Any()) return;

            var artist = new Artist
            {
                Name = "Полка",
                ImagePath = "media/artists/photo_2025-12-27_16-24-12.jpg"
            };

            context.Artists.Add(artist);
            context.SaveChanges();

            var songRelativePath = "media/songs/test_song.mp3";
            var songFullPath = Path.Combine(webRootPath, songRelativePath);
            var duration = GetDuration(songFullPath);

            var song = new Song
            {
                Title = "ММС",
                Duration = duration,
                ArtistId = artist.Id,
                FilePath = songRelativePath,
                CoverPath = "media/covers/photo_2026-04-08_00-00-00.jpg"
            };

            context.Songs.Add(song);
            context.SaveChanges();
        }

        private static int GetDuration(string fullPath)
        {
            if (!File.Exists(fullPath)) return 0;

            using var file = TagLib.File.Create(fullPath);
            return (int)file.Properties.Duration.TotalSeconds;
        }
    }
}
