using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{
    public class LoginResponseDto
    {
        public UserResponseDto UserResponse { get; set; }

        public string Token { get; set; }
    }
}
