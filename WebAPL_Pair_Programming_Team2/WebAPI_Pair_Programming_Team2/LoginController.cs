using Dapper;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NLog;
using NLog.Web;



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
                    //ユーザ名, パスワードの一致チェック
                    var sql = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName AND Password = @Password";
                    int count = connection.QuerySingle<int>(sql, new
                    {
                        UserName = request.UserName,
                        Password = request.Password
                    });

                    //ユーザ名, パスワードが両方とも一致している行がある場合
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
                    //ユーザ名, パスワードが一致する行がない場合
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
                //例外エラー
                _logger.LogWarning($"通信失敗：例外エラー");
                return StatusCode(500, new{success = false,message = "サーバーエラーが発生しました。" });
            }
        }

        public class LoginInfo
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
    

}