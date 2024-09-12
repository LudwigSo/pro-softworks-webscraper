using Driving.Service.Jobs;
using Quartz;

namespace Driving.Service;

internal static class ServiceQuartzDi
{    
    internal static IServiceCollection AddServiceQuartz(this IServiceCollection services)
    {
        services.AddScoped<HaysWebscraperJob>();
        services.AddScoped<FreelanceDeWebscraperJob>();
        services.AddQuartz(config =>
        {
            config.ScheduleJob<HaysWebscraperJob>(trigger => 
                trigger
                    .WithIdentity("HaysWebscraperJob")
                    .WithCronSchedule("0 */1 * * * ?")
            );
            config.ScheduleJob<FreelanceDeWebscraperJob>(trigger =>
                trigger
                    .WithIdentity("FreelanceDeWebscraperJob")
                    .WithCronSchedule("0 25 */12 * * ?")
            );
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
        return services;
    }

    internal static IServiceCollection AddServiceQuartzForDebugging(this IServiceCollection services)
    {
        services.AddScoped<HaysWebscraperJob>();
        services.AddScoped<FreelanceDeWebscraperJob>();
        services.AddQuartz(config =>
        {
            config.ScheduleJob<HaysWebscraperJob>(trigger =>
                trigger
                    .WithIdentity("HaysWebscraperJob")
                    .WithCronSchedule("0 */1 * * * ?")
            );
            config.ScheduleJob<FreelanceDeWebscraperJob>(trigger =>
                trigger
                    .WithIdentity("FreelanceDeWebscraperJob")
                    .WithCronSchedule("0 */2 * * * ?")
            );
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
        return services;
    }
}