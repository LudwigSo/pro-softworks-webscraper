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
        return services;
    }
}