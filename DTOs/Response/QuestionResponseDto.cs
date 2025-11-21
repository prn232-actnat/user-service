using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{
    public class QuestionResponseDto
    {
        public Guid QuestionId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserSimpleDto Author { get; set; }
        public List<CommentResponseDto> Replies { get; set; } = new List<CommentResponseDto>();
    }
}
