using Microsoft.Data.Sqlite;
using System;
using Dapper;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



public class DatabaseInitializer
{
    private const string ConnectionString = "Data Source=MyDatabase.db";

    public static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            // 1. 接続を開く
            connection.Open();

            // 2. テーブル作成のSQL文（存在しない場合のみ作成する IF NOT EXISTS がポイント）
            string createTableSql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT UNIQUE,
                    CreatedAt TEXT NOT NULL
                );";

            // 3. Dapperの「Execute」メソッドでSQLを実行
            connection.Execute(createTableSql);

            Console.WriteLine("テーブルの確認・作成が完了しました。");
        }
    }
}