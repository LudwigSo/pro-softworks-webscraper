using Application.Ports;
using Driven.Webscraper.Proxy;
using Driven.Webscraper.Scraper;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Webscraper;

public static class WebscraperDi
{
    public static IServiceCollection AddWebscraper(this IServiceCollection services)
    {
        services.AddScoped<IWebscraperPort, WebscraperFactory>();
        services.AddSingleton<IProxyLoader, ProxyscrapeLoader>();
        services.AddSingleton<HttpHelper>();

        return services;
    }
}