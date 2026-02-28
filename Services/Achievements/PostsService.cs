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
                .Include(p => p.PostInteraction)
                .FirstOrDefaultAsync(p => p.Id == id);

            return  _mapper.Map<PostsDisplayDto>(post);
        }

        public async Task<List<PostsDisplayDto>> GetAllPostsAsync()
        {
            var posts = await _context.Posts
                .Where(p => p.IsVisible && p.DeletedByUserId == null)
                .Include(p => p.Author)
                .Include(p => p.PostInteraction)
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

        public async Task<Posts?> SoftDeletePostAsync(int id, int userId, string reason)
        {
            //var post = await _context.Posts.FindAsync(id);

            var post = await _context.Posts
               .Include(p => p.Author)
               .FirstOrDefaultAsync(p => p.Id == id);

            //if (post == null) return false;
            post.IsVisible = false;
            post.DeletedByUserId = userId;

            _context.PostModerationLog.Add(new PostModerationLog
            {
                EntityType = "Post",
                EntityId = id,
                Action = "SoftDelete",
                Reason = reason,
                ModeratedByUserId = userId,
                TargetUserId = post.UserId??0,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return post;
        }
        public async Task<IEnumerable<PostsDisplayDto>> GetAllVisiblePostsAsync()
        {
            var posts = await _context.Posts
               .Where(p => p.IsVisible && p.DeletedByUserId == null)
               .Include(p => p.Author)
               .Include(p => p.PostImages)
               .Include(p => p.PostTagMaps)
                   .ThenInclude(ptm => ptm.Tag)
      
               .Include(p => p.PostInteraction)
               .OrderByDescending(p => p.CreatedAt)
               .ToListAsync();
            return  _mapper.Map<List<PostsDisplayDto>>(posts);
        }
        public async Task<IEnumerable<PostsDisplayDto>> GetUserPostsHistoryAsync(int userId)
        {
            var posts = await _context.Posts
                .Where(p => p.UserId == userId) 
                .Include(p => p.Author)
                .Include(p => p.PostImages)
                .Include(p => p.PostTagMaps)
                    .ThenInclude(ptm => ptm.Tag)
                .Include(p => p.PostInteraction)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<PostsDisplayDto>>(posts);
        }
        public async Task<int> GenerateSystemPosts()
        {
          
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.UtcNow);
            DateTime now = DateTime.UtcNow;
            var expiredPosts = await _context.Posts
                .Where(p => p.IsSystemGenerated && p.ExpiresAt < now && p.IsVisible)
                .ToListAsync();

            foreach (var oldPost in expiredPosts)
            {
                oldPost.IsVisible = false;
            }
            var birthdayUsers = await _context.UserProfile
                .Where(up => up.Birthday.Month == todayDate.Month &&
                             up.Birthday.Day == todayDate.Day)
                .ToListAsync();

            int createdCount = 0;

            foreach (var user in birthdayUsers)
            {
                var birthdayPost = new Posts
                {
                    UserId = user.UserProfileId,
                    Title = "Happy Birthday! 🎂",
                    Description = $"Wishing {user.FirstName} a wonderful birthday today!",
                    CreatedAt = now,
                    ExpiresAt = now.AddDays(1), 
                    IsVisible = true,
                    IsSystemGenerated = true
                };

                _context.Posts.Add(birthdayPost);
                await _context.SaveChangesAsync();

              
                _context.PostTagMaps.Add(new PostTagMap
                {
                    PostId = birthdayPost.Id,
                    TagId = 4
                });

                _context.PostInteraction.Add(new PostInteraction
                {
                    PostId = birthdayPost.Id,
                    LastUpdatedAt = now
                });

                createdCount++;
            }

            await _context.SaveChangesAsync();
            return createdCount;
        }

        public async Task<int> GenerateAnniversaryPosts()
        {
            DateTime now = DateTime.UtcNow;
            DateOnly todayDate = DateOnly.FromDateTime(now);
            int currentYear = now.Year;

            var anniversaryUsers = await _context.UserProfile
                .Where(up => up.JoinDate.Month == todayDate.Month &&
                             up.JoinDate.Day == todayDate.Day &&
                             up.JoinDate.Year < currentYear)
                .ToListAsync();

            int createdCount = 0;

            var newPosts = new List<Posts>();
            var newInteractions = new List<PostInteraction>();
            var newTagMaps = new List<PostTagMap>();

            foreach (var user in anniversaryUsers)
            {
                int yearsCompleted = currentYear - user.JoinDate.Year;
                var anniversaryPost = new Posts
                {
                    UserId = user.UserProfileId,
                    Title = "Work Anniversary! 🎊",
                    Description = $"Congratulations to {user.FirstName} for completing {yearsCompleted} years! 🚀",
                    CreatedAt = now,
                    ExpiresAt = now.AddDays(1),
                    IsVisible = true,
                    IsSystemGenerated = true
                };

                newPosts.Add(anniversaryPost);
                createdCount++;
            }

            _context.Posts.AddRange(newPosts);
            await _context.SaveChangesAsync();

            foreach (var post in newPosts)
            {
                _context.PostTagMaps.Add(new PostTagMap { PostId = post.Id, TagId = 5 });
                _context.PostInteraction.Add(new PostInteraction { PostId = post.Id, LastUpdatedAt = now });
            }
            await _context.SaveChangesAsync();

            return createdCount;
        }

        public async Task<bool> ToggleReactionAsync(PostInteractionCreateUpdateDto dto, int userId)
        {
            var existingReaction = await _context.UserPostReaction
                .FirstOrDefaultAsync(r => r.PostId == dto.PostId &&
                                          r.UserId == userId &&
                                          r.ReactionType.ToLower() == dto.ReactionType.ToLower());

            var interaction = await _context.PostInteraction
                .FirstOrDefaultAsync(i => i.PostId == dto.PostId);

            if (interaction == null) return false;

            if (existingReaction != null)
            {
                _context.UserPostReaction.Remove(existingReaction);
                UpdateCounter(interaction, dto.ReactionType, -1);
            }
            else
            {
                var newReaction = new UserPostReaction
                {
                    PostId = dto.PostId,
                    UserId = userId,
                    ReactionType = dto.ReactionType
                };
                _context.UserPostReaction.Add(newReaction);
                UpdateCounter(interaction, dto.ReactionType, 1);
            }

            interaction.LastUpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }
        private void UpdateCounter(PostInteraction interaction, string type, int adjustment)
        {
            switch (type.ToLower())
            {
                case "like": interaction.LikeCount = Math.Max(0, interaction.LikeCount + adjustment); break;
                case "celebrate": interaction.CelebrateCount = Math.Max(0, interaction.CelebrateCount + adjustment); break;
                case "love": interaction.LoveCount = Math.Max(0, interaction.LoveCount + adjustment); break;
                case "insightful": interaction.InsightfulCount = Math.Max(0, interaction.InsightfulCount + adjustment); break;
            }
        }
    }
}