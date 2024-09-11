using Domain.CommandHandlers;
using Domain.Model;
using Quartz;

namespace Driving.Service.Jobs;

internal class HaysWebscraperJob(ScrapeAndProcessCommandHandler scrapeAndProcessCommandHandler) : IJob
{
    private ScrapeAndProcessCommandHandler ScrapeAndProcessCommandHandler { get; } = scrapeAndProcessCommandHandler ?? throw new ArgumentNullException(nameof(scrapeAndProcessCommandHandler));
    public async Task Execute(IJobExecutionContext context) => await ScrapeAndProcessCommandHandler.Handle(new ScrapeAndProcessCommand(ProjectSource.Hays));
}
