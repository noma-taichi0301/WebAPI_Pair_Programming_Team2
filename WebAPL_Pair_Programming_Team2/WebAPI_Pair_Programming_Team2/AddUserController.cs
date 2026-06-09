using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Dapper;

namespace WebAPL_Pair_Programming_Team2
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddUserController:ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginInfo request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "ユーザー名とパスワードを入力してください。"
                });
            }
        }
    }


    public class LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

