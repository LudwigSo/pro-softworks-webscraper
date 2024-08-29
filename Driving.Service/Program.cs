using Driving.Service;
using Microsoft.Extensions.Hosting;
using Quartz;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((cxt, services) =>
    {
        services.AddQuartz(config =>
        {
            config.ScheduleJob<HaysWebscraperJob>(trigger => trigger.WithCronSchedule("*/15 * * * *"));
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
    }).Build();

// will block until the last running job completes
await builder.RunAsync();
