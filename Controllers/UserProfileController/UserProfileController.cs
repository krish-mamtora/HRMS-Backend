using HRMS_Backend.Common.Enums;
using HRMS_Backend.Common.Responses;
using HRMS_Backend.Model.DtoUserProfile;
using HRMS_Backend.Services.ServiceUserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.UserProfileController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserProfileDisplayDto>>>> GetUserProfiles(
            [FromQuery] string? role)
        {
            IEnumerable<UserProfileDisplayDto> profiles;

            if (!string.IsNullOrWhiteSpace(role))
            {
                profiles = await _userProfileService.GetProfilesByRoleAsync(role);
            }
            else
            {
                profiles = await _userProfileService.GetAllUsersAsync();
            }

            var response = ApiResponse<IEnumerable<UserProfileDisplayDto>>.SuccessResponse(
                profiles,
                "User profiles fetched successfully",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<UserProfileDisplayDto>>> GetUserById(
            int id)
        {
            var user = await _userProfileService.GetUserByIdAsync(id);

            var response = ApiResponse<UserProfileDisplayDto>.SuccessResponse(
                user,
                "User profile fetched successfully",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }

        [HttpGet("team/{id:int}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserProfileDisplayDto>>>> GetUsersByManagerId(
            int id)
        {
            var users = await _userProfileService.GetUsersByManagerIdAsync(id);

            var response = ApiResponse<IEnumerable<UserProfileDisplayDto>>.SuccessResponse(
                users,
                "Team members fetched successfully",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserProfileDisplayDto>>> CreateUserProfile(
            [FromBody] UserProfileCreateUpdateDto dto)
        {
            var createdProfile = await _userProfileService.CreateUserAsync(dto);

            var response = ApiResponse<UserProfileDisplayDto>.SuccessResponse(
                createdProfile,
                "User profile created successfully",
                (int)ResponseCode.Created
            );

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdProfile.UserProfileId },
                response
            );
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateUserProfile(
            int id,
            [FromBody] UserProfileCreateUpdateDto dto)
        {
            await _userProfileService.UpdateUserAsync(id, dto);

            var response = ApiResponse<string>.SuccessResponse(
                "Profile updated successfully",
                "User profile updated successfully",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }

        [HttpGet("email/{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> GetUserEmailFromId(
            int id)
        {
            var email = await _userProfileService.GetUserEmailFromIdAsync(id);

            var response = ApiResponse<object>.SuccessResponse(
                new
                {
                    Email = email
                },
                "User email fetched successfully",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }
    }
}