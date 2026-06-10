using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient; 

namespace WebAPL_Pair_Programming_Team2
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        //ログの初期設定
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        //DBアドレス指定
        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UsersDb;Trusted_Connection=True;TrustServerCertificate=True;";

        [HttpPost]
        public IActionResult Login([FromBody] LoginInfo request)
        {
            _logger.LogInformation($"POSTリクエスト(ログイン)を受け取りました");
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning($"ログイン失敗: ユーザ名またはパスワードがNullになっています");
                return BadRequest(new
                {
                    success = false,
                    message = "ユーザー名とパスワードを入力してください。"
                });
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    Console.WriteLine("ユーザを検索します");
                    var sql = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName AND Password = @Password";
                    int count = connection.QuerySingle<int>(sql, new
                    {
                        UserName = request.UserName,
                        Password = request.Password
                    });

                    if (count > 0)
                    {
                        _logger.LogInformation($"ログインに成功しました。UserName={request.UserName}");
                        return Ok(new
                        {
                            success = true,
                            message = "ログインに成功しました！",
                            username = request.UserName
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"ログイン失敗: ユーザ名またはパスワードが間違っています。");
                        return Unauthorized(new
                        {
                            success = false,
                            message = "ユーザー名またはパスワードが間違っています。"
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                // 💡 コンソールにエラーの生メッセージ（原因）を表示するようにして、デバッグしやすくしました
                Console.WriteLine($"予期せぬエラーを検出: {ex.Message}");
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
    

}