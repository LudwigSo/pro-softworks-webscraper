using Application.CommandHandlers;
using Application.QueryHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDi
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ScrapeAndProcessCommandHandler>();
        services.AddScoped<TagCommandHandler>();

        services.AddScoped<ProjectQueryHandler>();
        services.AddScoped<TagQueryHandler>();

        return services;
    }
}