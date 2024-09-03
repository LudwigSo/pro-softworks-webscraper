using Application;
using Driven.Logging;
using Driven.Persistence.Postgres;
using Driven.RealtimeMessages.SignalR;
using Driven.Webscraper;
using Driving.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder()
    .AddAdapterRealtimeMessagesSignalR();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder = builder.ConfigureServices((context, services) =>
{
    services.AddPersistencePostgres(configuration)
        .AddCommandHandlers()
        .AddWebscraper()
        .AddLogger()
        .AddServiceQuartz();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<Domain.Ports.ILogger>();
    var databaseContext = scope.ServiceProvider.GetRequiredService<Context>();
    databaseContext.Database.Migrate();
    logger.LogInformation("Database migrated.");
}


// will block until the last running job completes
await app.RunAsync();