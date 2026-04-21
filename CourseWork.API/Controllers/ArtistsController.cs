using System.Security.Claims;
using CourseWork.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CourseWork.API.Controllers
{
    [ApiController]
    [Route("api/artists")]
    [Authorize]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly ISongService _songService;
        private readonly ILikedSongService _likedSongService;
        private readonly IWebHostEnvironment _env;

        public ArtistsController(IArtistService artistService, ISongService songService, ILikedSongService likedSongService, IWebHostEnvironment env)
        {
            _artistService = artistService;
            _songService = songService;
            _likedSongService = likedSongService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var artists = await _artistService.GetAllAsync();
            return Ok(artists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var artist = await _artistService.GetByIdAsync(id);
            if (artist is null) return NotFound();
            return Ok(artist);
        }

        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetSongs(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!);
            var songs = (await _songService.GetByArtistIdAsync(id)).ToList();
            var likedIds = (await _likedSongService.GetLikedSongsAsync(userId))
                .Select(s => s.Id)
                .ToHashSet();

            foreach (var song in songs)
                song.IsLiked = likedIds.Contains(song.Id);

            return Ok(songs);
        }

        [HttpGet("{id}/image")]
        [AllowAnonymous]
        public async Task<IActionResult> Image(int id)
        {
            var imagePath = await _artistService.GetImagePathAsync(id);
            if (imagePath is null) return NotFound();

            var relativePath = $"media/artists/{Path.GetFileName(imagePath)}";
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            new FileExtensionContentTypeProvider().TryGetContentType(fullPath, out var contentType);
            return PhysicalFile(fullPath, contentType ?? "image/jpeg");
        }
    }
}
