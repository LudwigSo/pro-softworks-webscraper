using Quartz;

namespace Driving.Service;

internal static class ServiceQuartzDi
{    
    internal static IServiceCollection AddServiceQuartz(this IServiceCollection services)
    {
        services.AddScoped<HaysWebscraperJob>();
        services.AddQuartz(config =>
        {
            config.ScheduleJob<HaysWebscraperJob>(trigger => 
                trigger
                    .WithIdentity("HaysWebscraperJob")
                    .WithCronSchedule("0 */10 * * * ?")
            );
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
        return services;
    }
}