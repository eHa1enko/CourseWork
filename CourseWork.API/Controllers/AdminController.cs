using CourseWork.API.Services;
using CourseWork.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly ISongService _songService;
        private readonly ICloudinaryService _cloudinary;

        public AdminController(IArtistService artistService, ISongService songService, ICloudinaryService cloudinary)
        {
            _artistService = artistService;
            _songService = songService;
            _cloudinary = cloudinary;
        }

        // ── ARTISTS ──────────────────────────────────────────────

        [HttpPost("artists")]
        public async Task<IActionResult> CreateArtist([FromForm] string name, IFormFile? image)
        {
            if (!IsAdmin()) return Forbid();
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Name is required.");

            string? imageUrl = null;
            if (image is not null)
            {
                await using var stream = image.OpenReadStream();
                imageUrl = await _cloudinary.UploadImageAsync(stream, image.FileName, "artists");
            }

            var artist = await _artistService.CreateAsync(name, imageUrl);
            return Ok(artist);
        }

        [HttpDelete("artists/{id:int}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            if (!IsAdmin()) return Forbid();

            var artist = await _artistService.GetByIdAsync(id);
            if (artist is null) return NotFound();

            var imageUrl = await _artistService.GetImagePathAsync(id);
            if (imageUrl is not null)
                await _cloudinary.DeleteImageAsync(imageUrl);

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

            using var audioMs = new MemoryStream();
            await audio.CopyToAsync(audioMs);
            audioMs.Position = 0;

            var duration = GetDurationFromStream(audioMs, audio.FileName);
            audioMs.Position = 0;

            var audioUrl = await _cloudinary.UploadAudioAsync(audioMs, audio.FileName);

            string? coverUrl = null;
            if (cover is not null)
            {
                await using var coverStream = cover.OpenReadStream();
                coverUrl = await _cloudinary.UploadImageAsync(coverStream, cover.FileName);
            }

            var song = await _songService.CreateAsync(title, artistId, audioUrl, coverUrl, duration);
            return Ok(song);
        }

        [HttpDelete("songs/{id:int}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            if (!IsAdmin()) return Forbid();

            var fileUrl = await _songService.GetFilePathAsync(id);
            var coverUrl = await _songService.GetCoverPathAsync(id);

            var deleted = await _songService.DeleteAsync(id);
            if (!deleted) return NotFound();

            if (fileUrl is not null) await _cloudinary.DeleteAudioAsync(fileUrl);
            if (coverUrl is not null) await _cloudinary.DeleteImageAsync(coverUrl);

            return NoContent();
        }

        // ── HELPERS ───────────────────────────────────────────────

        private bool IsAdmin()
        {
            var claim = User.FindFirst("isAdmin");
            return claim?.Value == "true";
        }

        private static int GetDurationFromStream(Stream stream, string fileName)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(fileName)}");
            try
            {
                using (var fs = System.IO.File.Create(tempPath))
                    stream.CopyTo(fs);

                using var tagFile = TagLib.File.Create(tempPath);
                return (int)tagFile.Properties.Duration.TotalSeconds;
            }
            catch { return 0; }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
            }
        }
    }
}
