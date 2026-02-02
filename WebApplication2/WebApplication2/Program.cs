using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Npgsql;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
using var connection = new NpgsqlConnection(connectionString);

/* ================================= */
/* Create（建立）*/
/* ================================= */
app.MapPost("/user", async ([FromBody] CreateUserRequest createUserRequest) =>
{
    using var connection = new NpgsqlConnection(connectionString);
    var sql = """
    INSERT INTO users(plan_name,planned_hours,actual_hours,abandoned)
    VALUES(@Plan_name,@Planned_hours,@Actual_hours,@Abandoned)
    """;

    var result = await connection.ExecuteScalarAsync<UserDbModel>(sql, createUserRequest);

    return Results.Created();
});

/* ================================= */
/* Read（查詢）*/
/* ================================= */
app.MapGet("/user/{id}", async ([FromRoute] int id) =>
{
    using var connection = new NpgsqlConnection(connectionString);
    var sql = """
    SELECT id, plan_name, planned_hours, actual_hours, abandoned
    FROM users
    WHERE id = @Id;
    """;

    var user = await connection.QueryFirstOrDefaultAsync<UserDbModel>(sql, new { Id = id });

    if (user is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(user);
});

/* ================================= */
/* Read（查詢全部）*/
/* ================================= */
app.MapGet("/user", async () =>
{
    using var connection = new NpgsqlConnection(connectionString);
    var sql = """
    SELECT *
    FROM users
    """;

    var user = await connection.QueryAsync<UserDbModel>(sql);

    if (user is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(user);
});

/* ================================= */
/* Update（更新）*/
/* ================================= */
//app.MapPut("/user/:{plan_name}/{id}", async ([FromBody] UpdatePlan_name UserPlan_name) =>
//{
//    using var connection = new NpgsqlConnection(connectionString);
//    var sql = """
//    UPDATE users
//    SET plan_name = @Plan_name
//    WHERE id = @Id
//    """;

//    var result = await connection.ExecuteScalarAsync<UserDbModel>(sql, UserPlan_name);

//    return Results.Ok(result);
//});

//app.MapPost("/user/{planned_hours}", async ([FromRoute] int hours) =>
//{
//    using var connection = new NpgsqlConnection(connectionString);
//    var sql = """
//    UPDATE users
//    SET planned_hours = hours
//    WHERE hours = @Hours;
//    """;

//    var result = await connection.ExecuteScalarAsync<UserDbModel>(sql, new { Hours = hours });

//    return Results.Ok(result);
//});

//app.MapPost("/user/{actual_hours}", async ([FromRoute] int hours) =>
//{
//    using var connection = new NpgsqlConnection(connectionString);
//    var sql = """
//    UPDATE users
//    SET actual_hours = hours
//    WHERE hours = @Hours;
//    """;

//    var result = await connection.ExecuteScalarAsync<UserDbModel>(sql, new { Hours = hours });

//    return Results.Ok(result);
//});

//app.MapPost("/user/{abandoned}", async ([FromRoute] bool b) =>
//{
//    using var connection = new NpgsqlConnection(connectionString);
//    var sql = """
//    UPDATE users
//    SET abandoned = b
//    WHERE b = @B;
//    """;

//    var result = await connection.ExecuteScalarAsync<UserDbModel>(sql, new { B = b });

//    return Results.Ok(result);
//});

///* ================================= */
///* Delete（刪除）*/
///* ================================= */
//app.MapPost("/user/{id}", async ([FromRoute] int id) =>
//{
//    using var connection = new NpgsqlConnection(connectionString);
//    var sql = """
//    DELETE FROM users
//    WHERE id = @Id;
//    """;

//    var result = await connection.ExecuteScalarAsync<UserDbModel>(sql, new { Id = id });

//    return Results.Created();
//});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

/// <summary>
/// 表單內容
/// </summary>
public class UserDbModel
{
    public required int Id { get; set; }
    public required string plan_name { get; set; }
    public required int planned_hours { get; set; }
    public required int actual_hours { get; set; }
    public required bool abandoned { get; set; }
}

/// <summary>
/// 建立表單
/// </summary>
public class CreateUserRequest
{
    public required string plan_name { get; set; }
    public required int planned_hours { get; set; }
    public required int actual_hours { get; set; }
    public required bool abandoned { get; set; }
}

/// <summary>
/// 更新表單 plan_name
/// </summary>
public class UpdatePlan_name
{
    public required int Id { get; set; }
    public required string plan_name { get; set; }
}