using AutoMapper;
using HRMS_Backend.Common.Exceptions;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.FixEntityUserProfile;
using HRMS_Backend.Model.DtoUserProfile;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.ServiceUserProfile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            MyDbContext context,
            IMapper mapper,
            ILogger<UserProfileService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserProfileDisplayDto>> GetAllUsersAsync()
        {
            var users = await _context.UserProfile
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserProfileDisplayDto>>(users);
        }

        public async Task<UserProfileDisplayDto> GetUserByIdAsync(int id)
        {
            var user = await _context.UserProfile
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserProfileId == id);

            if (user == null)
            {
                _logger.LogWarning(
                    "User profile not found for id: {UserId}",
                    id);

                throw new NotFoundException("User profile not found");
            }

            return _mapper.Map<UserProfileDisplayDto>(user);
        }

        public async Task<IEnumerable<UserProfileDisplayDto>> GetProfilesByRoleAsync(
            string role)
        {
            var profiles = await _context.Users
                .Where(u => u.Role == role)
                .Join(
                    _context.UserProfile,
                    user => user.Id,
                    profile => profile.UserProfileId,
                    (user, profile) => profile
                )
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserProfileDisplayDto>>(profiles);
        }

        public async Task<IEnumerable<UserProfileDisplayDto>> GetUsersByManagerIdAsync(
            int managerId)
        {
            var users = await _context.UserProfile
                .Where(x => x.ManagerId == managerId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserProfileDisplayDto>>(users);
        }

        public async Task<UserProfileDisplayDto> CreateUserAsync(
            UserProfileCreateUpdateDto createUserDto)
        {
            var user = _mapper.Map<UserProfile>(createUserDto);

            await _context.UserProfile.AddAsync(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User profile created successfully with id: {UserId}",
                user.UserProfileId);

            return _mapper.Map<UserProfileDisplayDto>(user);
        }

        public async Task UpdateUserAsync(
            int id,
            UserProfileCreateUpdateDto updateUserDto)
        {
            var user = await _context.UserProfile
                .FirstOrDefaultAsync(x => x.UserProfileId == id);

            if (user == null)
            {
                _logger.LogWarning(
                    "Update failed. User profile not found for id: {UserId}",
                    id);

                throw new NotFoundException("User profile not found");
            }

            _mapper.Map(updateUserDto, user);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User profile updated successfully for id: {UserId}",
                id);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.UserProfile
                .FirstOrDefaultAsync(x => x.UserProfileId == id);

            if (user == null)
            {
                _logger.LogWarning(
                    "Delete failed. User profile not found for id: {UserId}",
                    id);

                throw new NotFoundException("User profile not found");
            }

            _context.UserProfile.Remove(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User profile deleted successfully for id: {UserId}",
                id);
        }

        public async Task<bool> IsUserBannedAsync(int userId)
        {
            var user = await _context.UserProfile
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserProfileId == userId);

            if (user == null)
            {
                _logger.LogWarning(
                    "User not found while checking ban status for id: {UserId}",
                    userId);

                throw new NotFoundException("User not found");
            }

            return user.IsUserBanned;
        }

        public async Task<string> GetUserEmailFromIdAsync(int id)
        {
            var email = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Email)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning(
                    "Email not found for user id: {UserId}",
                    id);

                throw new NotFoundException("User email not found");
            }

            return email;
        }

        public async Task<string> GetGameInterestedByIdAsync(int id)
        {
            var sport = await _context.UserProfile
                .AsNoTracking()
                .Where(x => x.UserProfileId == id)
                .Select(x => x.FavouriteSport)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(sport))
            {
                _logger.LogWarning(
                    "Favourite sport not found for user id: {UserId}",
                    id);

                throw new NotFoundException("Favourite sport not found");
            }

            return sport;
        }
    }
}