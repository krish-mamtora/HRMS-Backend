using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Services.GameScheduling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.GameScheduling
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameSlotsController
    {
        private readonly IGameSlotService _service;
        public GameSlotsController(IGameSlotService service)
        {
            _service = service;
        }

        [HttpPost("{GamesId}")]
        public async Task<int?> GenerateGameSlotAsync(int GamesId)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
           return await _service.GenerateGameSlotAsync(GamesId , today);
        }
    }
}
