namespace CourseWork.Entities.Entities
{
    public class LikedSong
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int SongId { get; set; }
        public Song Song { get; set; } = null!;
    }
}
