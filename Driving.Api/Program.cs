using Driven.Persistence.Postgres;
using Domain;
using Application;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Driving.Api.Hubs;
using Serilog;
using Serilog.Events;
using Driven.Webscraper;
using Application.Ports;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("/app/logs/log.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .CreateLogger();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Add controllers and configure route convention
builder.Services.AddControllers(o =>
{
    o.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyRouteParameterTransformer()));
    o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError));
})
.AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Add services
builder.Services
    .AddProblemDetails()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddPersistencePostgres(configuration)
    .AddWebscraper()
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger: logger, dispose: true))
    .AddApplicationServices();

builder.Services.AddHealthChecks();
builder.Services.AddScoped<IRealtimeMessagesPort, ProjectHub>();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.MapHub<ProjectHub>("/projectHub");
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseRouting();
app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();


internal class SlugifyRouteParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        var valueStr = value?.ToString();
        return valueStr == null ? null : Regex.Replace(valueStr, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    }
}