using Domain.Services.Webscraper;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Webscraper;

public static class WebscraperDI
{
    public static IServiceCollection AddWebscraper(this IServiceCollection services)
    {
        services.AddScoped<IWebscraperPort, WebscraperFactory>();

        return services;
    }
}