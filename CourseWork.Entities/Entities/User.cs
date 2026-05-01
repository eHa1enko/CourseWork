namespace CourseWork.Entities.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public ICollection<LikedSong> LikedSongs { get; set; } = new List<LikedSong>();
    }
}
