using Driven.Persistence.Postgres;
using Application;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Driving.Api.Hubs;
using Serilog;
using Driven.Webscraper;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();


// Add custom services
builder.Services
    .AddPersistencePostgres(configuration)
    .AddWebscraper()
    .AddApplicationServices()
    .AddRealtimeMessagesAdapter();

// Add preconfigured services
builder.Services.AddControllers(o =>
{
    o.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyRouteParameterTransformer()));
    o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError));
})
.AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
builder.Services
    .AddProblemDetails()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger: logger, dispose: true));

builder.Services.AddSignalR();

// build and run app
var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.MapHub<ProjectHub>("/projectHub");
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