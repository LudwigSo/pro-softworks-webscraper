using Domain.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Logging;

public static class WebscraperDi
{
    public static IServiceCollection AddLogger(this IServiceCollection services)
    {
        services.AddScoped<ILogger, SerilogLogger>();

        return services;
    }
}