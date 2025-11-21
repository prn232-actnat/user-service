using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
        public string Timestamp { get; set; }
    }
}
