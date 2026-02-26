using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Achievements;
using HRMS_Backend.Model.Achievements;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Services.Achievements
{
    public class PostsService : IPostsService
    {

        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PostsService(MyDbContext context, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<PostsDisplayDto> CreatePostAsync(PostsCreateUpdateDto dto, int userId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var post = _mapper.Map<Posts>(dto);
                post.UserId = userId;
                post.CreatedAt = DateTime.UtcNow;
                post.IsVisible = true;
                post.IsSystemGenerated = false;

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                if (dto.Images != null && dto.Images.Count > 0)
                {
                    foreach (var file in dto.Images)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine("AchievementImages", uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        _context.PostImages.Add(new PostImages
                        {
                            PostId = post.Id,
                            ImagePath = uniqueFileName
                        });
                    }
                }

                _context.PostInteraction.Add(new PostInteraction { PostId = post.Id, LastUpdatedAt = DateTime.UtcNow });
                if (dto.TagIds != null && dto.TagIds.Any())
                {
                    foreach (var tagId in dto.TagIds)
                    {
                        _context.PostTagMaps.Add(new PostTagMap { PostId = post.Id, TagId = tagId });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await GetPostByIdAsync(post.Id);

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<PostsDisplayDto> GetPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.PostImages)
                .Include(p => p.PostTagMaps).ThenInclude(pt => pt.Tag)
                .Include(p => p.Interactions)
                .FirstOrDefaultAsync(p => p.Id == id);

            return  _mapper.Map<PostsDisplayDto>(post);
        }

        public async Task<List<PostsDisplayDto>> GetAllPostsAsync()
        {
            var posts = await _context.Posts
                .Where(p => p.IsVisible && p.DeletedByUserId == null)
                .Include(p => p.Author)
                .Include(p => p.Interactions)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<PostsDisplayDto>>(posts);
        }
        public async Task<bool> UpdatePostAsync(int id, PostsCreateUpdateDto dto)
        {
            var post = await _context.Posts.Include(p => p.PostTagMaps).FirstOrDefaultAsync(x => x.Id == id);
            if (post == null) return false;

            _mapper.Map(dto, post);
            post.UpdatedAt = DateTime.UtcNow;

            _context.PostTagMaps.RemoveRange(post.PostTagMaps);
            if (dto.TagIds != null)
            {
                foreach (var tagId in dto.TagIds)
                {
                    _context.PostTagMaps.Add(new PostTagMap { PostId = id, TagId = tagId });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SoftDeletePostAsync(int id, int userId, string reason)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return false;

            post.IsVisible = false;
            post.DeletedByUserId = userId;

            _context.PostModerationLog.Add(new PostModerationLog
            {
                EntityType = "Post",
                EntityId = id,
                Action = "SoftDelete",
                Reason = reason,
                ModeratedByUserId = userId,
                TargetUserId = post.UserId,
                CreatedAt = DateTime.UtcNow
            });

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<PostsDisplayDto>> GetAllVisiblePostsAsync()
        {
            var posts = await _context.Posts
                .Where(p => p.IsVisible && p.DeletedByUserId == null)
                .Include(p => p.Author)
                .Include(p => p.PostImages)
                .Include(p => p.PostTagMaps).ThenInclude(ptm => ptm.Tag)
                .Include(p => p.Interactions)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return  _mapper.Map<List<PostsDisplayDto>>(posts);
        }
        public async Task<bool> ToggleReactionAsync(PostInteractionCreateUpdateDto dto, int userId)
        {
            var interaction = await _context.PostInteraction
                .FirstOrDefaultAsync(i => i.PostId == dto.PostId);

            if (interaction == null) return false;
            int adjustment = dto.IsActive ? 1 : -1;

            switch (dto.ReactionType.ToLower())
            {
                case "like":
                    interaction.LikeCount = Math.Max(0, interaction.LikeCount + adjustment);
                    break;
                case "celebrate":
                    interaction.CelebrateCount = Math.Max(0, interaction.CelebrateCount + adjustment);
                    break;
                case "love":
                    interaction.LoveCount = Math.Max(0, interaction.LoveCount + adjustment);
                    break;
                case "insightful":
                    interaction.InsightfulCount = Math.Max(0, interaction.InsightfulCount + adjustment);
                    break;
                default:
                    return false;
            }
            interaction.LastUpdatedAt = DateTime.UtcNow;

            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
    }
}
