using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.Data;

namespace DATERP.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("Default");
        using (var connection = new Npgsql.NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            try
            {
                using (var command = new Npgsql.NpgsqlCommand("DELETE FROM \"AbpUserRoles\" WHERE \"UserId\" IN (SELECT \"Id\" FROM \"AbpUsers\" WHERE \"UserName\" = 'admin')", connection))
                {
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
                using (var command = new Npgsql.NpgsqlCommand("DELETE FROM \"AbpUsers\" WHERE \"UserName\" = 'admin'", connection))
                {
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
            catch (PostgresException ex) when (ex.SqlState == "42P01")
            {
                Log.Warning("Tables do not exist yet, skipping manual cleanup.");
            }
        }

        using (var application = await AbpApplicationFactory.CreateAsync<DATERPDbMigratorModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
        }))
        {
            await application.InitializeAsync();

            // FIX: Manual Migration Trigger
            try
            {
                Log.Information("Starting database migration...");
                using (var scope = application.ServiceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DATERP.EntityFrameworkCore.DATERPDbContext>();
                    await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync(dbContext.Database, cancellationToken);
                    Log.Information("Database migration completed successfully.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to migrate database!");
                throw;
            }

            await application.ServiceProvider
                .GetRequiredService<IDataSeeder>()
                .SeedAsync();

            await application.ShutdownAsync();

            _hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
