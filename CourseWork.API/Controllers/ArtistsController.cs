using System.Security.Claims;
using CourseWork.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public ArtistsController(IArtistService artistService, ISongService songService, ILikedSongService likedSongService)
        {
            _artistService = artistService;
            _songService = songService;
            _likedSongService = likedSongService;
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
    }
}
