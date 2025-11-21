using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request
{
    public class CreateQuestionRequestDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "Tiêu đề phải từ 10 đến 255 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [MinLength(20, ErrorMessage = "Nội dung phải có ít nhất 20 ký tự")]
        public string Content { get; set; }
    }
}