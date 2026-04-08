using System.Security.Claims;
using CourseWork.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.API.Controllers
{
    [ApiController]
    [Route("api/liked-songs")]
    [Authorize]
    public class LikedSongsController : ControllerBase
    {
        private readonly ILikedSongService _likedSongService;

        public LikedSongsController(ILikedSongService likedSongService)
        {
            _likedSongService = likedSongService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("sub")!);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var songs = await _likedSongService.GetLikedSongsAsync(GetUserId());
            return Ok(songs);
        }

        [HttpPost("{songId}")]
        public async Task<IActionResult> Like(int songId)
        {
            await _likedSongService.LikeAsync(GetUserId(), songId);
            return Ok();
        }

        [HttpDelete("{songId}")]
        public async Task<IActionResult> Unlike(int songId)
        {
            await _likedSongService.UnlikeAsync(GetUserId(), songId);
            return NoContent();
        }
    }
}
