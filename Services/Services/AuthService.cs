using AutoMapper;
using DataAccess.UnitOfWork;
using DTOs;
using DTOs.Request;
using DTOs.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Models;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(loginRequest.Email);

            if (user == null || user.PasswordHash != loginRequest.Password)
            {
                return null;
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (user.LastLoginDate != null)
            {
                var lastLogin = user.LastLoginDate.Value;

                int diff = today.DayNumber - lastLogin.DayNumber;

                if (diff == 1)
                {
                    user.CurrentStreak += 1;
                }
                else if (diff > 1)
                {
                    user.CurrentStreak = 1;
                }
            }
            else
            {
                user.CurrentStreak = 1;
            }

            user.LastLoginDate = today;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                UserResponse = _mapper.Map<UserResponseDto>(user)
            };
        }


        public async Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterRequestDto dto)
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return (false, "Email đã tồn tại trong hệ thống");
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var minAllowedDate = today.AddYears(-6);

            if (dto.DateOfBirth > minAllowedDate)
            {
                return (false, "Ngày sinh không hợp lệ. Người dùng phải ít nhất 6 tuổi.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = dto.Password,
                Role = "STUDENT",
                Gender = dto.Gender,
                CreatedAt = DateTime.UtcNow,
                DateOfBirth = dto.DateOfBirth,
                UpdatedAt = DateTime.UtcNow,
                LastLoginDate = DateOnly.FromDateTime(DateTime.UtcNow),
                CurrentStreak = 0
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Đăng ký tài khoản thành công");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateProfileAsync(UpdateProfileRequestDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
            if (user == null)
                return (false, "Không tìm thấy người dùng");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dto.DateOfBirth > today.AddYears(-6))
                return (false, "Ngày sinh không hợp lệ. Người dùng phải ít nhất 6 tuổi.");

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);

            await _unitOfWork.SaveChangesAsync();

            return (true, "Cập nhật thông tin thành công");
        }

        public async Task<(bool IsSuccess, string Message)> ChangePasswordAsync(ChangePasswordRequestDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
            if (user == null)
                return (false, "Không tìm thấy người dùng");

            if (user.PasswordHash != dto.CurrentPassword)
                return (false, "Mật khẩu hiện tại không đúng");

            user.PasswordHash = dto.NewPassword;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Đổi mật khẩu thành công");
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName ?? string.Empty),
                new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty),
                new Claim("Gender", user.Gender ?? string.Empty),
                new Claim("DateOfBirth", user.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty),
                new Claim("CreatedAt", user.CreatedAt?.ToString("O") ?? string.Empty),
                new Claim("UpdatedAt", user.UpdatedAt?.ToString("O") ?? string.Empty),
                new Claim("LastLoginDate", user.LastLoginDate?.ToString() ?? string.Empty),
                new Claim("CurrentStreak", user.CurrentStreak?.ToString() ?? string.Empty),
                new Claim("Premium", user.Premium?.ToString() ?? string.Empty),
                new Claim("Points", user.Points?.ToString() ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
