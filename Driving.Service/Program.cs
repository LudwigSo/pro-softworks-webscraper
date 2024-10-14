using Application;
using Driven.Persistence.Postgres;
using Driven.Webscraper;
using Driving.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;


var builder = Host.CreateDefaultBuilder();

var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("/app/logs/log.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .CreateLogger();

//.UseSerilog((context, services, configuration) => configuration
//    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
//    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
//    .MinimumLevel.Information()
//    .WriteTo.Console()
//    .WriteTo.File("/app/logs/log.txt",
//        rollingInterval: RollingInterval.Day,
//        rollOnFileSizeLimit: true)
//);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder = builder.ConfigureServices((context, services) =>
{
    services.AddPersistencePostgres(configuration)
        .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger: logger, dispose: true))
        .AddApplicationServices()
        .AddRealtimeMessagesSignalR()
        .AddWebscraper()
        .AddServiceQuartz();
});


var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//    var databaseContext = scope.ServiceProvider.GetRequiredService<Context>();
//    await databaseContext.Database.MigrateAsync();
//    scopedLogger.LogInformation("Database migrated.");
//}

// will block until the last running job completes
await app.RunAsync();

// TODO this is currently required to create migrations, find a different way and remove this!
public class ContextFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<Context>();
        optionsBuilder.UseNpgsql("Host=db;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword;");

        return new Context(optionsBuilder.Options);
    }
}