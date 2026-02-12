////using AutoMapper;
////using HRMS_Backend.Data;
////using HRMS_Backend.Model;
////using Microsoft.EntityFrameworkCore;

//using HRMS_Backend.Model;
//using Microsoft.AspNetCore.Identity;

//namespace HRMS_Backend.Services.User
//{
//    public class UserService : IUserService
//    {
//        private readonly UserManager<UserDto> _userManager;
//        public UserService(UserManager<UserDto> userManager)
//        {
//            _userManager = userManager;
//        }

//        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
//        {
//            return await Task.FromResult(_userManager.Users.ToList());
//        }
//    }
//}
