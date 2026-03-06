using HRMS_Backend.Entities.Achievements;
using HRMS_Backend.Model.Achievements;

namespace HRMS_Backend.Services.Achievements
{
    public interface ICommentService
    {
        Task<CommentsDisplayDto> CreateOrUpdateCommentAsync(int userId, CommentsCreateUpdateDto dto);
        Task<List<CommentsDisplayDto>> GetCommentsByPostIdAsync(int postId);
        Task<Comments?> DeleteCommentAsync(int id, int userId, bool isHr);
        Task<int> GetCommentsCountByPostIdAsync(int id);
        Task<List<CommentsDisplayDto>> GetUserCommentHistoryAsync(int userId);
        Task<bool> SoftDeleteOwnCommentAsync(int commentId, int userId);
        Task<(bool Success, string Message)> RestoreOwnCommentAsync(int commentId, int userId);
        Task<Comments> RestoreModeratedCommentAsync(int commentId, int hrUserId);
    }

}
