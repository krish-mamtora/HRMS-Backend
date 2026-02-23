using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Model.GameScheduling;
using HRMS_Backend.Services.GameScheduling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.GameScheduling
{
    //[Authorize(Roles ="HR")]
    [Route("api/[controller]")]
    [ApiController]
    public class GameConfigController : ControllerBase
    {

        private readonly IGameConfigService _service;
        public GameConfigController(IGameConfigService service) {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> CreateGameConfig([FromBody] GameConfigCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createConfig = await _service.AddGameConfigurationAsync(dto);
            return CreatedAtAction(nameof(getGameConfigById), new { id = createConfig.Id }, createConfig);

        }

        [HttpGet("{id}", Name = "getGameConfigById")]
        public async Task<IActionResult> getGameConfigById(int id)
        {
            var GameConfig = await _service.getGameConfigByIdAsync(id);
            if (GameConfig == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(GameConfig);
        }
    }
}
