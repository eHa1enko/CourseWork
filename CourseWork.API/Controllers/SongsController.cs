using System.Security.Claims;
using CourseWork.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.API.Controllers
{
    [ApiController]
    [Route("api/songs")]
    [Authorize]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly ILikedSongService _likedSongService;

        public SongsController(ISongService songService, ILikedSongService likedSongService)
        {
            _songService = songService;
            _likedSongService = likedSongService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!);
            var songs = (await _songService.GetAllAsync()).ToList();
            var likedIds = (await _likedSongService.GetLikedSongsAsync(userId))
                .Select(s => s.Id)
                .ToHashSet();

            foreach (var song in songs)
                song.IsLiked = likedIds.Contains(song.Id);

            return Ok(songs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var song = await _songService.GetByIdAsync(id);
            if (song is null) return NotFound();
            return Ok(song);
        }
    }
}
