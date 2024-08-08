using Microsoft.Extensions.DependencyInjection;
using Domain.Services.Webscraper;

namespace Domain;

public static class DomainDI
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<WebscraperService>();

        return services;
    }
}