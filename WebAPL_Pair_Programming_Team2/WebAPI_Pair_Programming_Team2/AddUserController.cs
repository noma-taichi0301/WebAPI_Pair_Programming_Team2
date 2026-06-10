using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace WebAPL_Pair_Programming_Team2
{
    [ApiController]
    [Route("api/users")]
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;

        private readonly string _connectionString = "Server=YOUR_SERVER_NAME;Database=YOUR_DB_NAME;Trusted_Connection=True;TrustServerCertificate=True;";

        public RegisterController(ILogger<RegisterController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult RegisterUser([FromBody] RegisterRequest request)
        {
            // 簡易入力チェック
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { success = false, message = "ユーザー名とパスワードは必須です。" });
            }
            //  ユーザ名の文字数チェック（8文字〜30文字）
            if (request.UserName.Length > 30)
            {
                return BadRequest(new { success = false, message = "ユーザー名は30文字以内で入力してください。" });
            }

            //  パスワードの文字数チェック（8文字〜30文字）
            if (request.Password.Length < 8 || request.Password.Length > 30)
            {
                return BadRequest(new { success = false, message = "パスワードは8文字以上30文字以内で入力してください。" });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // 重複チェックの処理
                    var checkSql = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName";
                    int existingCount = connection.QuerySingle<int>(checkSql, new { UserName = request.UserName });

                    if (existingCount > 0)
                    {
                        _logger.LogWarning($"ユーザー登録失敗: ユーザー名が重複しています。UserName={request.UserName}");

                        return StatusCode(409, new
                        {
                            success = false,
                            message = "このユーザー名はすでに使用されています。別の名前を入力してください。"
                        });
                    }

                    // INSERTを実行
                    var insertSql = @"
                    INSERT INTO Users (UserName, Password) 
                    OUTPUT INSERTED.UserID
                    VALUES (@UserName, @Password);";

                    int newUserID = connection.QuerySingle<int>(insertSql, new
                    {
                        UserName = request.UserName,
                        Password = request.Password
                    });

                    _logger.LogInformation($"ユーザー登録成功: UserName={request.UserName}, Id={newUserID}");

                    return StatusCode(201, new
                    {
                        success = true,
                        message = "ユーザーを登録しました。",
                        masterId = newUserID
                    });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "ユーザー登録中にサーバーエラーが発生しました。");
                return StatusCode(500, new { success = false, message = "サーバーエラーが発生しました。" });
            }
        }
    } // 🟢 RegisterController クラスの終わり

    // 5. フロント（JS）からのJSONを受け取るためのクラスを同じ名前空間の中に並べる
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}