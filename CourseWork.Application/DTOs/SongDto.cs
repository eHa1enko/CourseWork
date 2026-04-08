namespace CourseWork.Application.DTOs
{
    public class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string? CoverUrl { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public bool IsLiked { get; set; }
    }
}
