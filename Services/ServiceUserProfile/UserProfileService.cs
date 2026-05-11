using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.FixEntityUserProfile;

//using HRMS_Backend.Model.UserProfile;
using HRMS_Backend.Model.DtoUserProfile;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace HRMS_Backend.Services.ServiceUserProfile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public UserProfileService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //public async Task<IEnumerable<UserProfileDisplayDto>> GetAllUsersAsync()
        //{
        //    var users = await _context.UserProfile.ToListAsync();
        //    return _mapper.Map<IEnumerable<UserProfileDisplayDto>>(users);
        //}
        public async Task<IEnumerable<UserProfileDisplayDto>> GetAllUsersAsync()
        {
            var users = await _context.UserProfile
                .Select(u => new UserProfileDisplayDto
                {
                    UserProfileId = u.UserProfileId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Address = u.Address,
                    Gender = u.Gender,
                    ManagerId = u.ManagerId,
                    ManagerName = _context.UserProfile
                        .Where(m => m.UserProfileId == u.ManagerId)
                        .Select(m => m.FirstName + " " + m.LastName)
                        .FirstOrDefault() ?? "No Manager",
                    Designation = u.Designation,
                    Birthday = u.Birthday,
                    Age = u.Age,
                    Department = u.Department,
                    FavouriteSport = u.FavouriteSport,
                    JoinDate = u.JoinDate,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return users;
        }


        public async Task<string?> getUserEmailfromId(int id)
        {
            var email = await _context.Users.Where(u => u.Id == id).Select(u => u.Email).FirstOrDefaultAsync();
            return email;
        }
        public async Task<UserProfileDisplayDto> GetUserByIdAsync(int id)
        {
            var user = await _context.UserProfile.FindAsync(id);
            return _mapper.Map<UserProfileDisplayDto>(user);
        }
        public async Task<string> GetGameInterestedByIdAsync(int id)
        {
            var sport = await _context.UserProfile.Where(u => u.UserProfileId == id).Select(u => u.FavouriteSport).FirstOrDefaultAsync();
            return sport;
        }
        public async Task<IEnumerable<UserProfile>> GetProfilesByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role == role) // Filter by role in Users table
                .Join(_context.UserProfile,
                      user => user.Id,            // Users table Primary Key (UserId)
                      profile => profile.UserProfileId,  // UserProfile table Foreign Key
                      (user, profile) => profile) // Select the profile data (Names, etc.)
                .ToListAsync();
        }


        public async Task<IEnumerable<UserProfileDisplayDto>> GetUsersByManagerIdAsync(int id)
        {
            var users = await _context.UserProfile.Where(up=>up.ManagerId==id).ToListAsync();
            return _mapper.Map<IEnumerable<UserProfileDisplayDto>>(users);
        }
        public async Task<UserProfileDisplayDto> CreateUserAsync(UserProfileCreateUpdateDto createUserDto)
        {
            var user = new HRMS_Backend.Entities.FixEntityUserProfile.UserProfile
            {
                IsActive = createUserDto.IsActive,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Gender = createUserDto.Gender,
                Designation = createUserDto.Designation,
                Address = createUserDto.Address,
                Age = createUserDto.Age,
                Department = createUserDto.Department,
                ManagerId = createUserDto.ManagerId,
                UserProfileId = createUserDto.UserProfileId,
            };

            _context.UserProfile.Add(user);
            await _context.SaveChangesAsync();

            return new UserProfileDisplayDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Address = user.Address,
                Designation = user.Designation,
                Age = user.Age,
                Department = user.Department,
                ManagerId = user.ManagerId,
                UserProfileId = user.UserProfileId,
                IsActive = user.IsActive,
            };

        }
        public async Task<bool> UpdateUserAsync(int id, UserProfileCreateUpdateDto updateUserDto)
        {
            var user = await _context.UserProfile.FindAsync(id);
            if (user == null) return false;

            _mapper.Map(updateUserDto, user);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"{ex.Message}");
                return false;
            }
        }

        public async Task<Boolean> IsUserBannedAsync(int userId)
        {
            var user = await _context.UserProfile.FirstOrDefaultAsync(u => u.UserProfileId == userId);

            if (user == null)
            {
                throw new Exception("User not found");

            }
            return user.IsUserBanned;
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.UserProfile.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            _context.UserProfile.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
