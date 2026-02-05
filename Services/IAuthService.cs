using HRMS_Backend.Entities;
using HRMS_Backend.Model;

namespace HRMS_Backend.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(UserDto request);
        Task<User?> RegisterAsync(UserDto request);
    }
}