using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.Sqlite;

namespace WebAPL_Pair_Programming_Team2
{
    [ApiController]
    [Route("api/login")]
    public class AddUserController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=users.db";

        [HttpPost]
        public IActionResult Login([FromBody] LoginInfo request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "ユーザー名とパスワードを入力してください。"
                });
            }
            try
            {
                // データベース（Dapper）でユーザーを探す
                using (var connection = new SqliteConnection(_connectionString))
                {
                    var sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password";
                    int count = connection.QuerySingle<int>(sql, new
                    {
                        UserName = request.UserName,
                        Password = request.Password
                    });

                    if (count > 0)
                    {
                        // Ok() を使うと、自動的に「ステータスコード 200」になります
                        return Ok(new
                        {
                            success = true,
                            message = "ログインに成功しました！",
                            username = request.UserName
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "ユーザー名またはパスワードが間違っています。"
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "サーバー側で予期せぬエラーが発生しました。時間を置いてやり直してください。"
                });
            }


        }


        public class LoginInfo
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
    public class LoginController
    {
    }
}
