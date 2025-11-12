using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    [Authorize(Roles = "STUDENT")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public string GetAll()
        {
            return "Hello";
        }
    }
}
