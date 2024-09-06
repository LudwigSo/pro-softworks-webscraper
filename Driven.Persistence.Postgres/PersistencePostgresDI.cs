using Domain.Ports;
using Domain.Ports.Queries;
using Driven.Persistence.Postgres.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Driven.Persistence.Postgres;

public static class PersistencePostgresDi
{
    public static IServiceCollection AddPersistencePostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<IWriteContext, DbWriteContext>();
        services.AddScoped<IProjectQueriesPort, ProjectQueries>();
        services.AddScoped<ITagQueriesPort, TagQueries>();

        return services;
    }
}