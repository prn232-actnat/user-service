using DTOs.Request;
using DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IForumService
    {
        Task<IEnumerable<QuestionResponseDto>> GetQuestionsAsync();
        Task<QuestionResponseDto> CreateQuestionAsync(CreateQuestionRequestDto dto, Guid userId);
        Task<CommentResponseDto> CreateCommentAsync(CreateCommentRequestDto dto, Guid userId);
        Task<bool> LikeQuestionAsync(Guid questionId, Guid userId);
        Task<bool> LikeCommentAsync(Guid commentId, Guid userId);
    }
}
