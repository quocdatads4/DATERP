using DATERP.Web;
using Serilog;
using Serilog.Events;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    // Debug login issues
    .MinimumLevel.Override("Microsoft.AspNetCore.Identity", LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Debug)
    .MinimumLevel.Override("Volo.Abp.Account", LogEventLevel.Debug)
    .MinimumLevel.Override("Volo.Abp.Identity", LogEventLevel.Debug)
    .MinimumLevel.Override("Education.Pages.Account", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File($"Logs/DATERP_{DateTime.Now:yyyyMMdd_HHmmss}.txt"))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

try
{
    Log.Information("Starting web host.");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseAutofac()
        .UseSerilog();

    await builder.AddApplicationAsync<DATERPWebModule>();
    var app = builder.Build();
    await app.InitializeApplicationAsync();
    await app.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
