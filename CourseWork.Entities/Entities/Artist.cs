namespace CourseWork.Entities.Entities
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
