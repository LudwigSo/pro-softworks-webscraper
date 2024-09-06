// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using Domain;
using Domain.CommandHandlers;
using Domain.Model;
using Domain.Ports;
using Driven.Logging;
using Driven.Logging.Serilog;
using Driven.Persistence.Postgres;
using Driven.Webscraper;
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

var webscraperFactory = serviceProvider.GetRequiredService<IWebscraperPort>();
await webscraperFactory.Scrape(ProjectSource.FreelanceDe);
