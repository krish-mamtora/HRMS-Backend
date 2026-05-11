using HRMS_Backend.Common.Constants;
using HRMS_Backend.Common.Enums;
using HRMS_Backend.Common.Responses;
using HRMS_Backend.Model;
using HRMS_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<object>>> Register(
            [FromBody] UserDto request)
        {
            var user = await _authService.RegisterAsync(request);

            var response = ApiResponse<object>.SuccessResponse(
                new
                {
                    user.Email
                },
                "Registration successful",
                (int)ResponseCode.Created
            );

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Login(
            [FromBody] UserDto request)
        {
            var token = await _authService.LoginAsync(request);

            var response = ApiResponse<TokenResponseDto>.SuccessResponse(
                token,
                "Login successful",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<TokenResponseDto>>> RefreshToken(
            [FromBody] RefreshTokenRequestDto request)
        {
            var token = await _authService.RefreshTokenAsync(request);

            var response = ApiResponse<TokenResponseDto>.SuccessResponse(
                token,
                "Token refreshed successfully",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }

        [HttpGet("auth-check")]
        [Authorize]
        public ActionResult<ApiResponse<string>> AuthCheck()
        {
            var response = ApiResponse<string>.SuccessResponse(
                "Authorized",
                "User authenticated successfully",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }

        [HttpGet("admin-check")]
        [Authorize(Roles = Roles.Admin)]
        public ActionResult<ApiResponse<string>> AdminCheck()
        {
            var response = ApiResponse<string>.SuccessResponse(
                "Authorized",
                "Admin authenticated successfully",
                (int)ResponseCode.Success
            );

            return Ok(response);
        }
    }
}