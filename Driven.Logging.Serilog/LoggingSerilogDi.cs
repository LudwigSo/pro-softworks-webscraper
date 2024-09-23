using Application.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Logging.Serilog;

public static class LoggingSerilogDi
{
    public static IServiceCollection AddLoggingSerilog(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddSingleton<ILogging>(l => new SerilogLogger(configuration));

        return services;
    }
}