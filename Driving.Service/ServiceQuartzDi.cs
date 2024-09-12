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
                    .WithCronSchedule("0 */16 * * * ?")
            );
            config.ScheduleJob<FreelanceDeOnlyNewWebscraperJob>(trigger =>
                trigger
                    .WithIdentity("FreelanceDeOnlyNewWebscraperJob")
                    .WithCronSchedule("0 */12 * * * ?")
            );
            config.ScheduleJob<FreelanceDeWebscraperJob>(trigger =>
                trigger
                    .WithIdentity("FreelanceDeWebscraperJob")
                    .WithCronSchedule("0 25 */12 * * ?")
            );
            config.ScheduleJob<FreelancerMapOnlyNewWebscraperJob>(trigger =>
                trigger
                    .WithIdentity("FreelancerMapOnlyNewWebscraperJob")
                    .WithCronSchedule("0 */10 * * * ?")
            );
            config.ScheduleJob<FreelancerMapWebscraperJob>(trigger =>
                trigger
                    .WithIdentity("FreelancerMapWebscraperJob")
                    .WithCronSchedule("0 25 */11 * * ?")
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