using Domain.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class DomainDi
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ScrapeAndProcessCommandHandler>();
        services.AddScoped<ReTagCommandHandler>();

        return services;
    }
}