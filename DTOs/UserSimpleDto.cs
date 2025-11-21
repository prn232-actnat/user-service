using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class UserSimpleDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string AvatarFallback { get; set; }
    }
}
