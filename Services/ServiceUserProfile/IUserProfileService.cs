using HRMS_Backend.Model.DtoUserProfile;

namespace HRMS_Backend.Services.ServiceUserProfile
{
    public interface IUserProfileService
    {
        Task<IEnumerable<UserProfileDisplayDto>> GetAllUsersAsync();

        Task<UserProfileDisplayDto> GetUserByIdAsync(int id);

        Task<UserProfileDisplayDto> CreateUserAsync(
            UserProfileCreateUpdateDto createUserDto);

        Task<IEnumerable<UserProfileDisplayDto>> GetUsersByManagerIdAsync(
            int managerId);

        Task UpdateUserAsync(
            int id,
            UserProfileCreateUpdateDto updateUserDto);

        Task DeleteUserAsync(int id);

        Task<IEnumerable<UserProfileDisplayDto>> GetProfilesByRoleAsync(
            string role);

        Task<bool> IsUserBannedAsync(int userId);

        Task<string> GetUserEmailFromIdAsync(int id);

        Task<string> GetGameInterestedByIdAsync(int id);
    }
}