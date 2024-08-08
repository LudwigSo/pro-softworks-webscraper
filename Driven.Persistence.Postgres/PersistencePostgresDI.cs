using Domain;
using Domain.Services.Webscraper;
using Driven.Persistence.Postgres.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Persistence.Postgres;

public static class PersistencePostgresDI
{
    public static IServiceCollection AddPersistencePostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<ICrudUnitOfWork, CrudUnitOfWork>();
        services.AddScoped<IProjectQueries, ProjectQueries>();

        return services;
    }
}