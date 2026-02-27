using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.Achievements;
using HRMS_Backend.Mapper;
using HRMS_Backend.Model.Achievements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Org.BouncyCastle.Asn1.Cms;

namespace HRMS_Backend.Services.Achievements
{
    public class CommentService : ICommentService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public CommentService(MyDbContext context,  IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommentsDisplayDto> CreateOrUpdateCommentAsync(int userId, CommentsCreateUpdateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            Comments? comment = null;

            if (dto.Id.HasValue && dto.Id > 0)
            {
                //comment = await _context.Comments
                //         .FirstOrDefaultAsync(c => c.Id == dto.Id && c.AuthorId == userId && !c.IsDeleted);

                //if (comment == null) throw new Exception("Comment not found or unauthorized.");

                //comment.Comment = dto.CommentText;
                //comment.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                comment = new Comments
                {
                    PostsId = dto.PostId,
                    AuthorId = userId,
                    Comment = dto.CommentText,
                    ParentCommentId = dto.ParentCommentId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Comments.Add(comment);
                 await _context.SaveChangesAsync();
            }


            var result = await GetCommentByIdAsync(comment.Id);
            return result!;
        }


        public async Task<CommentsDisplayDto> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
            .Include(c => c.Author) 
            .Include(c => c.Replies.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.Author)
            .FirstOrDefaultAsync(c => c.Id == id);  
            return _mapper.Map<CommentsDisplayDto>(comment);
        }


        public async Task<List<CommentsDisplayDto>> GetCommentsByPostIdAsync(int postId)
        {
            var comments = await _context.Comments
                .Where(c => c.PostsId == postId && c.ParentCommentId == null && !c.IsDeleted)
                .Include(c => c.Author)
                .Include(c => c.Replies.Where(r => !r.IsDeleted)).ThenInclude(r => r.Author)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<CommentsDisplayDto>>(comments);
        }

        public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.AuthorId != userId) return false;

            comment.IsDeleted = true;
            comment.DeletedByUserId = userId;
            comment.UpdatedAt = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
