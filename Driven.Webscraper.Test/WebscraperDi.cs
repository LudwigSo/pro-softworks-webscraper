using Domain.Ports;
using Driven.Webscraper.Test;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Webscraper;

public static class WebscraperDi
{
    public static IServiceCollection AddWebscraperForDebugging(this IServiceCollection services)
    {
        services.AddScoped<IWebscraperPort, WebscraperForDebugging>();

        return services;
    }
}