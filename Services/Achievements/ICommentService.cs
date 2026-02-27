using HRMS_Backend.Entities.Achievements;
using HRMS_Backend.Model.Achievements;

namespace HRMS_Backend.Services.Achievements
{
    public interface ICommentService
    {
        Task<CommentsDisplayDto> CreateOrUpdateCommentAsync(int userId, CommentsCreateUpdateDto dto);
        Task<List<CommentsDisplayDto>> GetCommentsByPostIdAsync(int postId);
        Task<bool> DeleteCommentAsync(int commentId, int userId);
    }
}
