using DTOs.Request;
using DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using WebAPI.CustomResponse;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    [Authorize(Roles = "STUDENT, TEACHER")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
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
