using Driven.Persistence.Postgres;
using Application;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Driving.Api.Hubs;
using Serilog;
using Driven.Webscraper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
    .AddSwaggerGenWithAuth(configuration)
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger: logger, dispose: true));

builder.Services.AddSignalR();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => 
    { 
        o.RequireHttpsMetadata = false;
        o.Audience = configuration["Keycloak:Audience"]; 
        o.MetadataAddress = configuration["Keycloak:MetadataAddress"]!;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Keycloak:ValidIssuer"],
        };
    });

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

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();

internal class SlugifyRouteParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        var valueStr = value?.ToString();
        return valueStr == null ? null : Regex.Replace(valueStr, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    }
}

internal static class SwaggerGenExtension
{
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(s =>
        {
            s.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            s.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" },
                        }
                    }
                }
            });

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme,
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer"
                    },
                    []
                }
            };

            s.AddSecurityRequirement(securityRequirement);
            //s.SwaggerDoc("v1", new() { Title = "Driving.Api", Version = "v1" });
        });
        return services;
    }
}