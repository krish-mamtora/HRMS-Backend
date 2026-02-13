using HRMS_Backend.Model.UserProfile;

namespace HRMS_Backend.Services.ServiceUserProfile
{
    public interface IUserProfileService
    {
        Task<IEnumerable<UserProfileDisplayDto>> GetAllUsersAsync();
        Task<UserProfileDisplayDto> GetUserByIdAsync(int id);

        Task<UserProfileDisplayDto> CreateUserAsync(UserProfileCreateUpdateDto createUserDto);
        Task<bool> UpdateUserAsync(int id, UserProfileCreateUpdateDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
    }
}
