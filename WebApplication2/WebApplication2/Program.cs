using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5174") // 你的 Vue 地址
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowVue");

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
    SELECT id, plan_name, planned_hours, actual_hours, abandoned
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
app.MapPut("/user", async ([FromQuery(Name = "id")] int id, [FromBody] UpdateUserRequest update) =>
{
    using var connection = new NpgsqlConnection(connectionString);
    var sql = """
    UPDATE users
    SET plan_name = @Name, planned_hours = @pHours, actual_hours = @aHours, abandoned = @Abandoned
    WHERE id = @Id
    """;

    await connection.ExecuteScalarAsync(sql, new
    {
        Id = id,
        Name = update.plan_name != "" ? update.plan_name : "無輸入內容",
        pHours = update.planned_hours,
        aHours = update.actual_hours,
        Abandoned = update.abandoned
    });

    return Results.Ok();
});

app.MapPut("/user/{id}", async ([FromRoute] int id, [FromBody] UpdateUserRequest update) =>
{
    using var connection = new NpgsqlConnection(connectionString);

    var sql = """
    UPDATE users
    SET plan_name = @Name, planned_hours = @pHours, actual_hours = @aHours, abandoned = @Abandoned
    WHERE id = @Id
    """;

    var result = await connection.ExecuteScalarAsync<UserDbModel>(sql, new
    {
        Id = id,
        Name = update.plan_name != null ? update.plan_name : "無輸入內容",
        pHours = update.planned_hours != null ? update.planned_hours : 0,
        aHours = update.actual_hours != null ? update.actual_hours : 0,
        Abandoned = update.abandoned != null ? update.abandoned : false
    });

    return Results.Ok(result);
});

/* ================================= */
/* Delete（刪除）*/
/* ================================= */
app.MapDelete("/user", async ([FromQuery(Name = "id")] int id) =>
{
    using var connection = new NpgsqlConnection(connectionString);
    var sql = """
    DELETE FROM users
    WHERE id = @Id;
    """;

    var result = await connection.ExecuteScalarAsync<UserDbModel>(sql, new { Id = id });

    return Results.Ok(result);
});

app.Run();

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
/// 更新表單
/// </summary>
public class UpdateUserRequest
{
    public required string? plan_name { get; set; }
    public required int? planned_hours { get; set; }
    public required int? actual_hours { get; set; }
    public required bool? abandoned { get; set; }
}