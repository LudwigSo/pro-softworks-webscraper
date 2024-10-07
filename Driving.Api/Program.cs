using Driven.Persistence.Postgres;
using Driven.Logging.Serilog;
using Domain;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Application;
using Driving.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

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
    .AddLoggingSerilog(configuration)
    .AddApplicationServices()
    .AddSignalR();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();
app.MapHub<ProjectHub>("/projecthub");
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