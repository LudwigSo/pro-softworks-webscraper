using Domain.CommandHandlers;
using Domain.Model;
using Quartz;

namespace Driving.Service.Jobs;

[DisallowConcurrentExecution]
internal class FreelancerMapOnlyNewWebscraperJob(ScrapeAndProcessCommandHandler scrapeAndProcessCommandHandler) : IJob
{
    private ScrapeAndProcessCommandHandler ScrapeAndProcessCommandHandler { get; } = scrapeAndProcessCommandHandler ?? throw new ArgumentNullException(nameof(scrapeAndProcessCommandHandler));
    public async Task Execute(IJobExecutionContext context) => await ScrapeAndProcessCommandHandler.Handle(new ScrapeAndProcessOnlyNewCommand(ProjectSource.FreelancerMap));
}