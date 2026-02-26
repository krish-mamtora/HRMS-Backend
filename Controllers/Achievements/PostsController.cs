using HRMS_Backend.Model.Achievements;
using HRMS_Backend.Services.Achievements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS_Backend.Controllers.Achievements
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;

        public PostsController(IPostsService postsService)
        {
            _postsService = postsService;
        }
        [HttpPost("upsert")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpsertPost([FromForm] PostsCreateUpdateDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized("User ID not found in token.");
            }

            try{    
                if (dto.Id.HasValue && dto.Id > 0)
                {
                    var success = await _postsService.UpdatePostAsync(dto.Id.Value, dto);
                    return success ? Ok("Post Updated Successfully") : BadRequest("Update Failed");
                 }
                else
                {
                    var post = await _postsService.CreatePostAsync(dto, currentUserId);
                    return Ok(post);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<PostsDisplayDto>>> GetAllPosts()
        {
            var posts = await _postsService.GetAllVisiblePostsAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostsDisplayDto>> GetPost(int id)
        {
            var post = await _postsService.GetPostByIdAsync(id);
            if (post == null) return NotFound("Post not found");
            return Ok(post);
        }
        [HttpPost("react")]
        public async Task<IActionResult> ToggleReaction([FromBody] PostInteractionCreateUpdateDto dto , int userId)
        {
            var result = await _postsService.ToggleReactionAsync(dto, userId);
            return result ? Ok("Reaction Updated") : BadRequest("Could not process reaction");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id, [FromQuery] int userId , string reason)
        {
            var result = await _postsService.SoftDeletePostAsync(id, userId, reason);
            return result ? Ok("Post Deleted Successfully") : NotFound("Post not found or already deleted");
        }
    }
}
