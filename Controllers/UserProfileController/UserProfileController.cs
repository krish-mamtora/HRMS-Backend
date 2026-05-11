using HRMS_Backend.Entities;
using HRMS_Backend.Entities.FixEntityUserProfile;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Model;
using HRMS_Backend.Model.DtoUserProfile;
using HRMS_Backend.Services.ServiceUserProfile;
using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.UserProfileController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {

        private readonly IUserProfileService _service;

        public UserProfileController(IUserProfileService service)
        {
            _service = service;
        }
        //[HttpGet]
        //public async Task<ActionResult> GetAllUsers()
        //{
        //    var users = await _service.GetAllUsersAsync();
        //    return Ok(users);
        //}
       
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UserProfileCreateUpdateDto dto)
        //{
        //    var success = await _service.UpdateUserAsync(id, dto);

        //    if (!success) { 
        //        return BadRequest(new { message = "Update failed or user not found" }); 
        //    }
        //    return Ok(new { message = "User updated successfully" });
        //}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles([FromQuery] string? role)
        {
            if (!string.IsNullOrEmpty(role))
            {
                var profilesByRole = await _service.GetProfilesByRoleAsync(role);
                return Ok(profilesByRole);
            }

            var allProfiles = await _service.GetAllUsersAsync();
            return Ok(allProfiles);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _service.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("team/{id}", Name = "GetUsersByManagerIdAsync")]
        public async Task<ActionResult> GetUsersByManagerIdAsync(int id)
        {
            var users = await _service.GetUsersByManagerIdAsync(id);
            return Ok(users);
        }
        [HttpPost]
        public async Task<ActionResult> CreateuserProfile([FromBody] UserProfileCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createProfile = await _service.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = createProfile.UserProfileId }, createProfile);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UserProfileCreateUpdateDto dto)
        {
            if(!ModelState.IsValid)
            { 
                return BadRequest(ModelState); 
            }

            var result = await _service.UpdateUserAsync(id, dto);

            if (!result)
            {
                return NotFound(new { message = "Could not update profile." });
            }

            return Ok(new { message = "Profile updated successfully!!!!" });
        }
        [HttpGet("getusetemailfromId/{id}")]
        public async Task<IActionResult> GetUserEmailfromId(int id)
        {
            var email = await _service.getUserEmailfromId(id);

            if (string.IsNullOrEmpty(email))
            {
                return NotFound(new { message = "Email address not found for this employee." });
            }
            return Ok(new { email });
        }
        
    }
}
