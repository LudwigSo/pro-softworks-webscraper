// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net;
using Domain;
using Domain.CommandHandlers;
using Domain.Model;
using Domain.Ports;
using Driven.Logging;
using Driven.Logging.Serilog;
using Driven.Persistence.Postgres;
using Driven.Webscraper;
using Driven.Webscraper.Proxy;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Setup configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Setup dependency injection
var serviceCollection = new ServiceCollection()
    //.AddPersistencePostgres(configuration)
    .AddDomainServices()
    .AddWebscraper()
    .AddLoggingSerilog();

var serviceProvider = serviceCollection.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger>();

//var proxy = new WebProxy("52.226.125.25", 8080);
//var httpClientHandler = new HttpClientHandler
//{
//    Proxy = proxy,
//    UseProxy = true,
//};

//var httpClient = new HttpClient(httpClientHandler);


//using HttpResponseMessage response = await httpClient.GetAsync("http://httpbin.org/ip");
//string responseContent = await response.Content.ReadAsStringAsync();
//Console.WriteLine(responseContent);


var httpHelper = serviceProvider.GetRequiredService<HttpHelper>();
var tasks = new List<Task>();
for (var i = 0; i < 50; i++)
{
    tasks.Add(
        Task.Run(async () =>
        {
            var html = await httpHelper.GetHtml("http://www.wieistmeineip.de/");
            if (html == null)
            {
                Console.WriteLine("Failed to get html");
                return;
            }
            var result = html.DocumentNode.SelectSingleNode("//div[@id='ipv4']/div[@class='title']")?.InnerText;
            if (result == null)
            {
                Console.WriteLine($"Failed to find ip node: {html.ParsedText}");
                return;
            }
            Console.WriteLine(result);
        })
    );
}

Task.WaitAll(tasks.ToArray());
Console.WriteLine("Done!");


//var webscraperFactory = serviceProvider.GetRequiredService<IWebscraperPort>();
//await webscraperFactory.Scrape(ProjectSource.FreelanceDe);
