using HRMS_Backend.Services.GameScheduling;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.GameScheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController
    {
        private readonly IGamesService _service;
        public GameController(IGamesService service) {
            _service = service;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Games>>> GetGames()
        //{
        //    var games = await _service.GetAllGamesAsync();
        //    return Ok(games);
        //}
    }
}
