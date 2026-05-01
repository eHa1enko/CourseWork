using CourseWork.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.API.Controllers
{
    [ApiController]
    [Route("api/search")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly ISongService _songService;

        public SearchController(IArtistService artistService, ISongService songService)
        {
            _artistService = artistService;
            _songService = songService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Ok(new { artists = Array.Empty<object>(), songs = Array.Empty<object>() });

            var artists = await _artistService.SearchAsync(q);
            var songs = await _songService.SearchAsync(q);
            return Ok(new { artists, songs });
        }
    }
}
