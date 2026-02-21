//using HRMS_Backend.Model;
//using HRMS_Backend.Services.User;
//using Microsoft.AspNetCore.Mvc;

//namespace HRMS_Backend.Controllers.User
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersController : ControllerBase
//    {
       
//            private readonly IUserService _userService;

//            public UsersController(IUserService userService)
//            {
//                _userService = userService;
//            }

//            [HttpGet]
//            public async Task<IActionResult> GetAll()
//            {
//                var users = await _userService.GetAllUsersAsync();
//                return Ok(users);
//            }

//    }
//}
