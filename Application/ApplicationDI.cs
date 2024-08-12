using Application.Webscraper;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DomainDI
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ScrapeAndProcessCommandHandler>();

        return services;
    }
}