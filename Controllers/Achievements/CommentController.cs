using HRMS_Backend.Model.Achievements;
using HRMS_Backend.Services.Achievements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace HRMS_Backend.Controllers.Achievements
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<ActionResult<CommentsDisplayDto>> UpsertComment([FromBody] CommentsCreateUpdateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

                int userId = int.Parse(userIdClaim);
                var result = await _commentService.CreateOrUpdateCommentAsync(userId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("post/{postId}")]
        public async Task<ActionResult<List<CommentsDisplayDto>>> GetCommentsByPost(int postId)
        {
            var results = await _commentService.GetCommentsByPostIdAsync(postId);
            return Ok(results);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id, int userId)
        {
            var idDeleted = await _commentService.DeleteCommentAsync(id, userId);
            if (!idDeleted) return NotFound("Comment not found or unauthorized.");

            return NoContent();
        }
    }
}
