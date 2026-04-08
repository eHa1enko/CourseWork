namespace CourseWork.Entities.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? CoverPath { get; set; }

        public int ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;

        public ICollection<LikedSong> LikedBy { get; set; } = new List<LikedSong>();
    }
}
