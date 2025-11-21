using AutoMapper;
using DataAccess.UnitOfWork;
using DTOs;
using DTOs.Request;
using DTOs.Response;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Services.IServices;
using System.Security.Claims;

namespace Services.Services
{
    public class ForumService : IForumService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ForumService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetQuestionsAsync()
        {
            var questions = await _unitOfWork.Questions.GetQueryable()
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            var questionIds = questions.Select(q => q.Id).ToList();

            var allComments = await _unitOfWork.Comments.GetQueryable()
                .Where(c => questionIds.Contains(c.QuestionId))
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            var questionDtos = _mapper.Map<List<QuestionResponseDto>>(questions);
            var commentDtos = _mapper.Map<List<CommentResponseDto>>(allComments);

            var commentMap = commentDtos.ToDictionary(c => c.CommentId);
            var nestedComments = new List<CommentResponseDto>();

            foreach (var commentDto in commentDtos)
            {
                if (commentDto.ParentCommentId.HasValue && commentMap.TryGetValue(commentDto.ParentCommentId.Value, out var parentComment))
                {
                    parentComment.Replies.Add(commentDto);
                }
                else
                {
                    nestedComments.Add(commentDto);
                }
            }

            var commentsByQuestionId = nestedComments.GroupBy(c => c.QuestionId);

            foreach (var questionDto in questionDtos)
            {
                var questionComments = commentsByQuestionId.FirstOrDefault(g => g.Key == questionDto.QuestionId);
                if (questionComments != null)
                {
                    questionDto.Replies.AddRange(questionComments);
                }
            }

            return questionDtos;
        }

        public async Task<QuestionResponseDto> CreateQuestionAsync(CreateQuestionRequestDto dto, Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new Exception("Không tìm thấy người dùng.");

            var question = new Question
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Content = dto.Content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Likes = 0
            };

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<QuestionResponseDto>(question);
            resultDto.Author = _mapper.Map<UserSimpleDto>(user);
            return resultDto;
        }

        public async Task<CommentResponseDto> CreateCommentAsync(CreateCommentRequestDto dto, Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new Exception("Không tìm thấy người dùng.");

            var question = await _unitOfWork.Questions.GetByIdAsync(dto.QuestionId);
            if (question == null) throw new Exception("Không tìm thấy câu hỏi.");

            if (dto.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.Comments.GetByIdAsync(dto.ParentCommentId.Value);
                if (parentComment == null) throw new Exception("Không tìm thấy bình luận cha.");
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = dto.Content,
                QuestionId = dto.QuestionId,
                ParentCommentId = dto.ParentCommentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Likes = 0
            };

            await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<CommentResponseDto>(comment);
            resultDto.Author = _mapper.Map<UserSimpleDto>(user);
            return resultDto;
        }

        public async Task<bool> LikeQuestionAsync(Guid questionId, Guid userId)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
            if (question == null)
                return false; 

            var existingLike = await _unitOfWork.QuestionLikes.GetAsync(
                ql => ql.QuestionId == questionId && ql.UserId == userId
            );

            if (existingLike != null)
            {
                _unitOfWork.QuestionLikes.Delete(existingLike);
                question.Likes -= 1;
            }
            else
            {
                var newLike = new QuestionLike
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    QuestionId = questionId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.QuestionLikes.AddAsync(newLike);
                question.Likes += 1; 
            }

            _unitOfWork.Questions.Update(question);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikeCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            if (comment == null)
                return false; 

            var existingLike = await _unitOfWork.CommentLikes.GetAsync(
                cl => cl.CommentId == commentId && cl.UserId == userId
            );

            if (existingLike != null)
            {
                _unitOfWork.CommentLikes.Delete(existingLike);
                comment.Likes -= 1;
            }
            else
            {
                var newLike = new CommentLike
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CommentId = commentId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.CommentLikes.AddAsync(newLike);
                comment.Likes += 1;
            }

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}