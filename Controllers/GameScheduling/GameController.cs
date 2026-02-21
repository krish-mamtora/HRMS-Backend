using HRMS_Backend.Services.GameScheduling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.GameScheduling
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController
    {
        private readonly IGamesService _service;
        public GameController(IGamesService service) {
            _service = service;
        }

      
    }
}
