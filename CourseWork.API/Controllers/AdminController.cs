using CourseWork.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagLib;

namespace CourseWork.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly ISongService _songService;
        private readonly IWebHostEnvironment _env;

        public AdminController(IArtistService artistService, ISongService songService, IWebHostEnvironment env)
        {
            _artistService = artistService;
            _songService = songService;
            _env = env;
        }

        // ── ARTISTS ──────────────────────────────────────────────

        [HttpPost("artists")]
        public async Task<IActionResult> CreateArtist([FromForm] string name, IFormFile? image)
        {
            if (!IsAdmin()) return Forbid();
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Name is required.");

            string? imagePath = null;
            if (image is not null)
            {
                imagePath = await SaveFile(image, "media/artists");
            }

            var artist = await _artistService.CreateAsync(name, imagePath);
            return Ok(artist);
        }

        [HttpDelete("artists/{id:int}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            if (!IsAdmin()) return Forbid();

            var artist = await _artistService.GetByIdAsync(id);
            if (artist is null) return NotFound();

            // delete artist image file
            var imagePath = await _artistService.GetImagePathAsync(id);
            if (imagePath is not null)
                DeleteFile(imagePath);

            var deleted = await _artistService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        // ── SONGS ─────────────────────────────────────────────────

        [HttpPost("songs")]
        public async Task<IActionResult> CreateSong(
            [FromForm] string title,
            [FromForm] int artistId,
            IFormFile audio,
            IFormFile? cover)
        {
            if (!IsAdmin()) return Forbid();
            if (string.IsNullOrWhiteSpace(title)) return BadRequest("Title is required.");
            if (audio is null) return BadRequest("Audio file is required.");

            var audioPath = await SaveFile(audio, "media/songs");
            var audioFullPath = Path.Combine(_env.WebRootPath, audioPath);
            var duration = GetDuration(audioFullPath);

            string? coverPath = null;
            if (cover is not null)
                coverPath = await SaveFile(cover, "media/covers");

            var song = await _songService.CreateAsync(title, artistId, audioPath, coverPath, duration);
            return Ok(song);
        }

        [HttpDelete("songs/{id:int}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            if (!IsAdmin()) return Forbid();

            var filePath = await _songService.GetFilePathAsync(id);
            var coverPath = await _songService.GetCoverPathAsync(id);

            var deleted = await _songService.DeleteAsync(id);
            if (!deleted) return NotFound();

            if (filePath is not null) DeleteFile(filePath);
            if (coverPath is not null) DeleteFile(coverPath);

            return NoContent();
        }

        // ── HELPERS ───────────────────────────────────────────────

        private bool IsAdmin()
        {
            var claim = User.FindFirst("isAdmin");
            return claim?.Value == "true";
        }

        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var relativePath = $"{folder}/{fileName}";
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return relativePath;
        }

        private void DeleteFile(string relativePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }

        private static int GetDuration(string fullPath)
        {
            if (!System.IO.File.Exists(fullPath)) return 0;
            using var file = TagLib.File.Create(fullPath);
            return (int)file.Properties.Duration.TotalSeconds;
        }
    }
}
