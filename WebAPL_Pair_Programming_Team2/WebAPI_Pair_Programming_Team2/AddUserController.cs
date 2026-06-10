using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NLog;
using NLog.Web;

namespace WebAPL_Pair_Programming_Team2
{

    //ユーザ追加機能
    [ApiController]
    [Route("api/users")]
    public class AddUserController : ControllerBase
    {
        //ログの初期設定
        private readonly ILogger<AddUserController> _logger;
        public AddUserController(ILogger<AddUserController> logger)
        {
            _logger = logger;
        }

        //DBアドレス指定
        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UsersDb;Trusted_Connection=True;TrustServerCertificate=True;";

        [HttpPost]
        public IActionResult AddUser([FromBody] User request)
        {
            _logger.LogInformation($"POSTリクエスト(ユーザ追加)を受け取りました");
            // 簡易入力チェック
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning($"ユーザー登録失敗: ユーザ名またはパスワードがNullになっています");
                return BadRequest(new { success = false, message = "ユーザー名とパスワードは必須です。" });
            }
            //  ユーザ名の文字数チェック（8文字〜30文字）
            if (request.UserName.Length > 30)
            {
                _logger.LogWarning($"ユーザー登録失敗: ユーザ名が長すぎます");
                return BadRequest(new { success = false, message = "ユーザー名は30文字以内で入力してください。" });
            }

            //  パスワードの文字数チェック（8文字〜30文字）
            if (request.Password.Length < 8 || request.Password.Length > 30)
            {
                _logger.LogWarning($"ユーザー登録失敗: パスワードが短すぎるか長すぎます");
                return BadRequest(new { success = false, message = "パスワードは8文字以上30文字以内で入力してください。" });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // 重複チェックの処理
                    var checkSql = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName";
                    int existingCount = connection.QuerySingle<int>(checkSql, new { UserName = request.UserName });

                    //重複のあった場合の処理
                    if (existingCount > 0)
                    {
                        _logger.LogWarning($"ユーザー登録失敗: ユーザー名が重複しています。UserName={request.UserName}");

                        return StatusCode(409, new
                        {
                            success = false,
                            message = "このユーザー名はすでに使用されています。別の名前を入力してください。"
                        });
                    }

                    // 重複がなかった場合の処理
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
            //例外処理
            catch (System.Exception ex)
            {
                _logger.LogWarning($"通信失敗：例外エラー");
                return StatusCode(500, new { success = false, message = "サーバーエラーが発生しました。" });
            }
        }
    }




    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}