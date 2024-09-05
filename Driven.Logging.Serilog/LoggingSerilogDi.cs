using Domain.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Logging.Serilog;

public static class LoggingSerilogDi
{
    public static IServiceCollection AddLoggingSerilog(this IServiceCollection services)
    {
        services.AddScoped<ILogger, SerilogLogger>();

        return services;
    }
}