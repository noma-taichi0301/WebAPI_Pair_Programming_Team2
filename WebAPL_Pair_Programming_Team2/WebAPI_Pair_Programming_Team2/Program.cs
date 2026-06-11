using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using Microsoft.AspNetCore.Identity.Data;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlServer(
        "Server=(localdb)\\MSSQLLocalDB;Database=UsersDb;Trusted_Connection=True;TrustServerCertificate=True"));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    db.Database.EnsureCreated();
}
// Add services to the container.



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



//DB作成
public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
}


public class User
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
public class Chat
{
    public int ChatID { get; set; }
    public string UserName { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}


namespace WebAPL_Pair_Programming_Team2
{
    [ApiController]
    [Route("api/chats")]
    public class PostChatController : ControllerBase
    {
        //ログの初期設定
        private readonly ILogger<AddUserController> _logger;
        public PostChatController(ILogger<AddUserController> logger)
        {
            _logger = logger;
        }
        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UsersDb;Trusted_Connection=True;TrustServerCertificate=True;";

        [HttpPost]
        public IActionResult Chat([FromBody] Chat request)
        {
            _logger.LogInformation($"POSTリクエスト(チャット)を受け取りました");
            if (string.IsNullOrEmpty(request.Message))
            {
                _logger.LogWarning($"メッセージが空の状態でpostされました");
                return BadRequest(new
                {
                    success = false,
                    message = "メッセージを入力してください"
                });
            }
            if (request.Message.Length>400)
            {
                _logger.LogWarning($"400文字を超えるメッセージが投稿されました");
                return BadRequest(new
                {
                    success = false,
                    message = "メッセージが長すぎます"
                });
            }


            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var insertSql = @"
                    INSERT INTO Chats (UserName, Message, CreatedAt) 
                    OUTPUT INSERTED.ChatID
                    VALUES (@UserName, @Message, @CreatedAt);";

                    int newChatID = connection.QuerySingle<int>(insertSql, new
                    {
                        UserName = request.UserName,
                        Message = request.Message,
                        CreatedAt = DateTime.Now
                    });
                    _logger.LogInformation($"チャット保存成功: Message={request.Message}, Id={newChatID}");

                    return StatusCode(201, new
                    {
                        success = true,
                        message = "チャットを送信しました。",
                        masterId = newChatID
                    });
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"予期せぬエラーを検出: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "サーバー側で予期せぬエラーが発生しました。時間を置いてやり直してください。"
                });
            }
        }
    }


    [ApiController]
    [Route("api/chats")]
    public class GetChatController : ControllerBase
    {
        private readonly ILogger<GetChatController> _logger;
        public GetChatController(ILogger<GetChatController> logger)
        {
            _logger = logger;
        }
        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UsersDb;Trusted_Connection=True;TrustServerCertificate=True;";

        [HttpGet]
        public IActionResult GetMessages()
        {
            _logger.LogInformation($"GETリクエスト(チャット)を受け取りました");
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ChatID, UserName, Message, CreatedAt FROM Chats ORDER BY CreatedAt ASC";
                    IEnumerable<Chat> messageList = connection.Query<Chat>(sql);
                    _logger.LogInformation($"通信成功：メッセージリストをクライアントに送信します。");
                    return Ok(messageList);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning($"通信失敗：例外エラー");
                return StatusCode(500, new { success = false, message = $"チャット履歴の取得に失敗しました: {ex.Message}" });
            }
        }
    }
}