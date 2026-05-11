using HRMS_Backend.Common.Constants;
using HRMS_Backend.Common.Exceptions;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.FixEntityUserProfile;
using HRMS_Backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HRMS_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(
            IConfiguration configuration,
            MyDbContext context,
            ILogger<AuthService> logger,
            IPasswordHasher<User> passwordHasher)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> RegisterAsync(UserDto request)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (emailExists)
            {
                throw new BadRequestException(
                    "Email already exists");
            }

            var user = new User
            {
                Email = request.Email,

                Role = Roles.Employee,

                UserProfile = new UserProfile
                {
                    FirstName = "New",
                    LastName = "User",
                    JoinDate = DateTime.UtcNow,
                    IsActive = true,
                    ManagerId = 1
                }
            };

            user.PasswordHash = _passwordHasher
                .HashPassword(user, request.Password);

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User registered successfully");

            return user;
        }

        public async Task<TokenResponseDto> LoginAsync(
            UserDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == request.Email);

            if (user is null)
            {
                _logger.LogWarning(
                    "Invalid login credentials");

                throw new UnauthorizedException(
                    "Invalid email or password");
            }

            var result = _passwordHasher
                .VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning(
                    "Invalid login credentials");

                throw new UnauthorizedException(
                    "Invalid email or password");
            }

            var tokenResponse = new TokenResponseDto
            {
                AccessToken = CreateToken(user),

                RefreshToken = await GenerateAndSaveRefreshToken(user),

                Role = user.Role,

                Id = user.Id.ToString()
            };

            _logger.LogInformation(
                "User login successful");

            return tokenResponse;
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(
            RefreshTokenRequestDto request)
        {
            var user = await _context.Users
                .FindAsync(request.userId);

            if (user is null ||
                user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning(
                    "Invalid refresh token");

                throw new UnauthorizedException(
                    "Invalid refresh token");
            }

            var tokenResponse = new TokenResponseDto
            {
                AccessToken = CreateToken(user),

                RefreshToken = await GenerateAndSaveRefreshToken(user),

                Role = user.Role,

                Id = user.Id.ToString()
            };

            return tokenResponse;
        }

        private async Task<string> GenerateAndSaveRefreshToken(
            User user)
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            var refreshToken =
                Convert.ToBase64String(randomNumber);

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiry =
                DateTime.UtcNow.AddDays(1);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var tokenKey =
                _configuration["AppSettings:Token"];

            if (string.IsNullOrWhiteSpace(tokenKey))
            {
                throw new Exception(
                    "JWT token key is missing");
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    user.Email),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _configuration["AppSettings:Issuer"],

                audience: _configuration["AppSettings:Audience"],

                claims: claims,

                expires: DateTime.UtcNow.AddHours(1),

                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}