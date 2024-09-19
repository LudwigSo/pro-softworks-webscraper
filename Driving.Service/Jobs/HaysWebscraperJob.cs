using Domain;
using Application.CommandHandlers;
using Quartz;

namespace Driving.Service.Jobs;

[DisallowConcurrentExecution]
internal class HaysWebscraperJob(ScrapeAndProcessCommandHandler scrapeAndProcessCommandHandler) : IJob
{
    private ScrapeAndProcessCommandHandler ScrapeAndProcessCommandHandler { get; } = scrapeAndProcessCommandHandler ?? throw new ArgumentNullException(nameof(scrapeAndProcessCommandHandler));
    public async Task Execute(IJobExecutionContext context) => await ScrapeAndProcessCommandHandler.Handle(new ScrapeAndProcessCommand(ProjectSource.Hays));
}
