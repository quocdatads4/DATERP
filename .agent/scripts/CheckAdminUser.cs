// Quick script to check if admin user exists in database
using Npgsql;

var connectionString = "Host=31.220.84.102;Port=5433;Database=daterp;Username=admin-tqd;Password=PgI_hOcs8NkK_Kp9jzLT_kz0;Trust Server Certificate=true";

try
{
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();
    Console.WriteLine("Connected to database successfully!");

    // Check all users
    await using var cmd = new NpgsqlCommand(@"
        SELECT ""Id"", ""UserName"", ""Email"", ""EmailConfirmed"", ""IsActive"", ""NormalizedEmail""
        FROM ""AbpUsers"" 
        LIMIT 20", conn);

    await using var reader = await cmd.ExecuteReaderAsync();

    Console.WriteLine("\n=== Admin Users Found ===");
    int count = 0;
    while (await reader.ReadAsync())
    {
        count++;
        Console.WriteLine($"ID: {reader["Id"]}");
        Console.WriteLine($"UserName: {reader["UserName"]}");
        Console.WriteLine($"Email: {reader["Email"]}");
        Console.WriteLine($"NormalizedEmail: {reader["NormalizedEmail"]}");
        Console.WriteLine($"EmailConfirmed: {reader["EmailConfirmed"]}");
        Console.WriteLine($"IsActive: {reader["IsActive"]}");
        Console.WriteLine("---");
    }

    if (count == 0)
    {
        Console.WriteLine("No admin users found!");
    }
    else
    {
        Console.WriteLine($"Total: {count} admin user(s) found.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
