using DTOs.Request;
using DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterRequestDto dto);
        Task<(bool IsSuccess, string Message)> UpdateProfileAsync(UpdateProfileRequestDto dto);
        Task<(bool IsSuccess, string Message)> ChangePasswordAsync(ChangePasswordRequestDto dto);
    }
}
