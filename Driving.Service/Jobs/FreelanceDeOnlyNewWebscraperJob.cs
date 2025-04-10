﻿using Domain;
using Application.CommandHandlers;
using Quartz;

namespace Driving.Service.Jobs;

[DisallowConcurrentExecution]
internal class FreelanceDeOnlyNewWebscraperJob(ScrapeAndProcessCommandHandler scrapeAndProcessCommandHandler) : IJob
{
    private ScrapeAndProcessCommandHandler ScrapeAndProcessCommandHandler { get; } = scrapeAndProcessCommandHandler ?? throw new ArgumentNullException(nameof(scrapeAndProcessCommandHandler));
    public async Task Execute(IJobExecutionContext context) => await ScrapeAndProcessCommandHandler.Handle(new ScrapeAndProcessCommand(ProjectSource.FreelanceDe));
}