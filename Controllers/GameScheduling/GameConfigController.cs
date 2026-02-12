using HRMS_Backend.Model.GameScheduling;
using HRMS_Backend.Services.GameScheduling;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.GameScheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameConfigController : ControllerBase
    {

        private readonly IGameConfigService _service;
        public GameConfigController(IGameConfigService service) {
            _service = service;
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateGameConfig([FromBody] GameConfigCreateUpdateDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var createConfig = _service.AddGameConfigurationAsync(dto);
        //    return CreatedAtAction(nameof(getGameConfigById), new { id = createConfig.Id }, createConfig);

        //}


        //[HttpGet("{id}", Name = "GetJobById")]
        //public async Task<IActionResult> getGameConfigById(int id) 
        //{
        //    var GameConfig = _service.
        //}
    }
}
