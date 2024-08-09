// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using Domain;
using Domain.Services.Webscraper;
using Driven.Persistence.Postgres;
using Driven.Webscraper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello World!");

while (!Debugger.IsAttached)
{
    Console.WriteLine("Waiting for debugger to attach...");
    await Task.Delay(100);
}
Console.WriteLine("Debugger attached.");

// Setup configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Setup dependency injection
var serviceCollection = new ServiceCollection()
    .AddPersistencePostgres(configuration)
    .AddDomainServices()
    .AddWebscraper();

var serviceProvider = serviceCollection.BuildServiceProvider();

serviceProvider.GetRequiredService<Context>().Database.Migrate();

var webscraper = serviceProvider.GetRequiredService<WebscraperService>();
await webscraper.ScrapeAndProcess();

Console.WriteLine("Bye World!");

public class ContextFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<Context>();
        optionsBuilder.UseNpgsql("Host=db;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword;");

        return new Context(optionsBuilder.Options);
    }
}

// var webscraperPort = new WebscraperFactory();
// var webscraperService = new WebscraperService(webscraperPort);

// var projects = await webscraperService.Scrape();

// Initialize Playwright
// using var playwright = await Playwright.CreateAsync();
// var browser = await playwright.Chromium.ConnectOverCDPAsync("ws://browserless:3000?token=094632bb-e326-4c63-b953-82b55700b14c&headless=false&stealth&record=true");
// var page = await browser.NewPageAsync();

// await page.GotoAsync("https://www.hays.de/jobsuche/stellenangebote-jobs?page=1&mrew=1&joblevel=3&mrew=1&q=&e=false");
// Console.WriteLine(await page.TitleAsync());



// await browser.CloseAsync();


// var serviceCollection = new ServiceCollection();
// var configuration = new ConfigurationBuilder()
//     .SetBasePath(Directory.GetCurrentDirectory())
//     .AddJsonFile("appsettings.json")
//     .Build();

// serviceCollection.AddDbContext<ApplicationDbContext>(options =>
//     options.UseNpgsql(
//         configuration.GetConnectionString("DefaultConnection")));

// var serviceProvider = serviceCollection.BuildServiceProvider();
// services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseNpgsql(
//         Configuration.GetConnectionString("DefaultConnection")));
