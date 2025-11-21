using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request
{
    public class CreateCommentRequestDto
    {
        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; }

        [Required]
        public Guid QuestionId { get; set; }

        public Guid? ParentCommentId { get; set; }
    }
}
