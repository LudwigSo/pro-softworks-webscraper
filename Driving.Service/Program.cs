using Application;
using Driven.Persistence.Postgres;
using Driven.Webscraper;
using Driving.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Serilog;


var builder = Host.CreateDefaultBuilder();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();


builder = builder.ConfigureServices((context, services) =>
{
    // add custom services
    services.AddPersistencePostgres(configuration)
        .AddApplicationServices()
        .AddRealtimeMessagesSignalR()
        .AddWebscraper()
        .AddServiceQuartz();

    // add preconfigured services
    var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
    services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders().AddSerilog(logger: logger, dispose: true));
});

// build and run app

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var databaseContext = scope.ServiceProvider.GetRequiredService<Context>();
    await databaseContext.Database.MigrateAsync();
    scopedLogger.LogInformation("Database migrated.");
}

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