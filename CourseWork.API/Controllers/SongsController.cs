using CourseWork.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CourseWork.API.Controllers
{
    [ApiController]
    [Route("api/songs")]
    [Authorize]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly IWebHostEnvironment _env;

        public SongsController(ISongService songService, IWebHostEnvironment env)
        {
            _songService = songService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var songs = await _songService.GetAllAsync();
            return Ok(songs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var song = await _songService.GetByIdAsync(id);
            if (song is null) return NotFound();
            return Ok(song);
        }

        [HttpGet("{id}/stream")]
        [AllowAnonymous]
        public async Task<IActionResult> Stream(int id)
        {
            var relativePath = await _songService.GetFilePathAsync(id);
            if (relativePath is null) return NotFound();

            var fullPath = Path.Combine(_env.WebRootPath, relativePath);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(fileStream, "audio/mpeg", enableRangeProcessing: true);
        }

        [HttpGet("{id}/cover")]
        [AllowAnonymous]
        public async Task<IActionResult> Cover(int id)
        {
            var relativePath = await _songService.GetCoverPathAsync(id);
            if (relativePath is null) return NotFound();

            var fullPath = Path.Combine(_env.WebRootPath, relativePath);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            new FileExtensionContentTypeProvider().TryGetContentType(fullPath, out var contentType);
            return PhysicalFile(fullPath, contentType ?? "image/jpeg");
        }
    }
}
