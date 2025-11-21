using DTOs.Request;
using DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.IServices;
using System.Security.Claims;
using WebAPI.CustomResponse;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/user/forum")]
    public class ForumController : ControllerBase
    {
        private readonly IForumService _forumService;

        public ForumController(IForumService forumService)
        {
            _forumService = forumService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực.");
            }
            return userId;
        }

        [HttpGet("questions")]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _forumService.GetQuestionsAsync();
            return Ok(ApiResponse<IEnumerable<QuestionResponseDto>>.Success(questions));
        }

        [HttpPost("questions")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var firstErrorMessage = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(ApiErrorResponses.ValidationError(firstErrorMessage));
            }

            var userId = GetCurrentUserId();

            var newQuestion = await _forumService.CreateQuestionAsync(request, userId);

            return CreatedAtAction(nameof(GetAllQuestions), new { id = newQuestion.QuestionId }, ApiResponse<QuestionResponseDto>.Success(newQuestion, ResponseMessage.RequestSuccessful));
        }

        [HttpPost("comments")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var firstErrorMessage = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(ApiErrorResponses.ValidationError(firstErrorMessage));
            }
            var userId = GetCurrentUserId();
            var newComment = await _forumService.CreateCommentAsync(request, userId);
            return Ok(ApiResponse<CommentResponseDto>.Success(newComment, ResponseMessage.RequestSuccessful));
        }

        [HttpPost("questions/{id}/like")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> LikeQuestion(Guid id)
        {
            var userId = GetCurrentUserId();
            var success = await _forumService.LikeQuestionAsync(id, userId);
            if (!success)
            {
                return NotFound(ApiErrorResponses.ResourceNotFound);
            }
            return Ok(ApiResponse<string>.Success("Like thành công"));
        }

        [HttpPost("comments/{id}/like")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> LikeComment(Guid id)
        {
            var userId = GetCurrentUserId();
            var success = await _forumService.LikeCommentAsync(id, userId);

            if (!success)
            {
                return NotFound(ApiErrorResponses.ResourceNotFound);
            }
            return Ok(ApiResponse<string>.Success("Like thành công"));
        }
    }
}
