using AdService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdsController : ControllerBase
    {
        private readonly IAdService _adService;

        public AdsController(IAdService adService)
        {
            _adService = adService;
        }

        /// <summary>
        /// Загружает рекламные площадки из файла (перезаписывает данные).
        /// </summary>
        [HttpPost("upload")]
        public IActionResult Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не загружен");

            using var stream = file.OpenReadStream();
            _adService.LoadFromFile(stream);

            return Ok("Данные загружены");
        }

        /// <summary>
        /// Ищет рекламные площадки по локации.
        /// </summary>
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string location)
        {
            var result = _adService.Search(location);
            return Ok(result);
        }
    }
}
