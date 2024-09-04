using Domain.CommandHandlers;
using Domain.Model;
using Quartz;

namespace Driving.Service;

internal class HaysWebscraperJob(ScrapeAndProcessCommandHandler scrapeAndProcessCommandHandler)
    : ProjectWebscraperJob(scrapeAndProcessCommandHandler)
{
    protected override ProjectSource Source { get; } = ProjectSource.Hays;
}

internal abstract class ProjectWebscraperJob(ScrapeAndProcessCommandHandler scrapeAndProcessCommandHandler)
    : IJob
{
    protected abstract ProjectSource Source { get; }
    private ScrapeAndProcessCommandHandler ScrapeAndProcessCommandHandler { get; } = scrapeAndProcessCommandHandler ?? throw new ArgumentNullException(nameof(scrapeAndProcessCommandHandler));

    public async Task Execute(IJobExecutionContext context)
    {
        var command = new ScrapeAndProcessCommand(Source);
        await ScrapeAndProcessCommandHandler.Handle(command);
    }
}