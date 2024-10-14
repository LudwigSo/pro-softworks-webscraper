using Application.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog;
using Microsoft.Extensions.Logging;

namespace Driven.Logging.Serilog;

public static class LoggingSerilogDi
{
    public static IServiceCollection AddLoggingSerilog(this IServiceCollection services, IConfigurationRoot configuration)
    {

         var logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("/app/logs/log.txt",
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true)
        .CreateLogger();

        services.AddScoped<ILogger>(logger);
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        return services;
    }
}