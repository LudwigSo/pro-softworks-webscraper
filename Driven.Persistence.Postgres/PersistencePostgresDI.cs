using Application.Ports;
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

        services.AddScoped<IWriteContext, DbWriteContext>();
        services.AddScoped<IReadContext, DbReadContext>();

        return services;
    }
}