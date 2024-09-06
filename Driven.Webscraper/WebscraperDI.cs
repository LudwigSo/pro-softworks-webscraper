using Domain.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Webscraper;

public static class WebscraperDi
{
    public static IServiceCollection AddWebscraper(this IServiceCollection services)
    {
        services.AddScoped<IWebscraperPort, WebscraperFactory>();

        return services;
    }
}