using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;


string? myVar = Environment.GetEnvironmentVariable("MY_VARIABLE");
string? anotherVar = Environment.GetEnvironmentVariable("ANOTHER_VARIABLE");

Console.WriteLine($"MY_VARIABLE: {myVar}");
Console.WriteLine($"ANOTHER_VARIABLE: {anotherVar}");


var builder = WebApplication.CreateBuilder(args);

// Read connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSingleton(new MySqlConnection(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var conn = scope.ServiceProvider.GetRequiredService<MySqlConnection>();
    try
    {
        await conn.OpenAsync();
        Console.WriteLine("Successfully connected to the database.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
    finally
    {
        await conn.CloseAsync();
    }
}

app.UseHttpsRedirection();

app.MapGet("/getAllTables", async (MySqlConnection conn) =>
{
    try
    {
        Console.WriteLine("getAllTalbe");
        await conn.OpenAsync();
        using var cmd = new MySqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        var tables = new List<string>();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }

        return Results.Ok(tables);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error fetching tables: {ex.Message}");
    }
    finally
    {
        await conn.CloseAsync();
    }
});

app.MapPost("/createTable/{tableName}", async (string tableName, MySqlConnection conn) =>
{
    try
    {
        await conn.OpenAsync();
        var query = $"CREATE TABLE `{tableName}` (id INT PRIMARY KEY AUTO_INCREMENT, Value INT)";
        using var cmd = new MySqlCommand(query, conn);
        await cmd.ExecuteNonQueryAsync();

        return Results.Ok($"Table '{tableName}' created successfully.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error creating table: {ex.Message}");
    }
    finally
    {
        await conn.CloseAsync();
    }
});

app.MapPost("/insertValue/{tableName}/{value}", async (string tableName, int value, MySqlConnection conn) =>
{
    try
    {
        await conn.OpenAsync();
        var query = $"INSERT INTO `{tableName}` (Value) VALUES (@Value)";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Value", value);
        await cmd.ExecuteNonQueryAsync();

        return Results.Ok($"Inserted value {value} into table '{tableName}'.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error inserting value: {ex.Message}");
    }
    finally
    {
        await conn.CloseAsync();
    }
});

app.MapGet("/getValues/{tableName}", async (string tableName, MySqlConnection conn) =>
{
    try
    {
        await conn.OpenAsync();
        var query = $"SELECT Value FROM `{tableName}`";
        using var cmd = new MySqlCommand(query, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        var values = new List<int>();
        while (await reader.ReadAsync())
        {
            values.Add(reader.GetInt32(0));
        }

        return Results.Ok(values);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error fetching values: {ex.Message}");
    }
    finally
    {
        await conn.CloseAsync();
    }
});

app.MapGet("/", () => "Hello from C#!");
app.Run();
