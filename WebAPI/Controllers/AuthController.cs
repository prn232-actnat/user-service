using DTOs;
using DTOs.Request;
using DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories.Models;
using Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.CustomResponse;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                var firstErrorMessage = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(ApiErrorResponses.ValidationError(firstErrorMessage));
            }

            var loginResponse = await _authService.LoginAsync(loginRequest);

            if (loginResponse == null)
            {
                return Unauthorized(ApiErrorResponses.ValidationError("Email hoặc Mật khẩu không hợp lệ"));
            }

            return Ok(ApiResponse<LoginResponseDto>.Success(loginResponse));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
            {
                var firstErrorMessage = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(ApiErrorResponses.ValidationError(firstErrorMessage));
            }

            var (IsSuccess, Message) = await _authService.RegisterAsync(registerRequest);

            if (!IsSuccess)
            {
                return BadRequest(ApiErrorResponses.ValidationError(Message));
            }

            return Ok(ApiResponse<string>.Success(Message));
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(ApiErrorResponses.ValidationError(firstError));
            }

            var result = await _authService.UpdateProfileAsync(request);

            if (!result.IsSuccess)
                return BadRequest(ApiErrorResponses.ValidationError(result.Message));

            return Ok(ApiResponse<string>.Success(result.Message));
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(ApiErrorResponses.ValidationError(firstError));
            }

            var result = await _authService.ChangePasswordAsync(request);

            if (!result.IsSuccess)
                return BadRequest(ApiErrorResponses.ValidationError(result.Message));

            return Ok(ApiResponse<string>.Success(result.Message));
        }
    }
}
