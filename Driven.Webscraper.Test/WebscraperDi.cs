using Domain.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Webscraper.Test;

public static class WebscraperDi
{
    public static IServiceCollection AddWebscraperForDebugging(this IServiceCollection services)
    {
        services.AddScoped<IWebscraperPort, WebscraperForDebugging>();

        return services;
    }
}