using Application.Ports;
using Domain;
using Driven.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.CommandHandlers;

public sealed record ScrapeAndProcessCommand(ProjectSource Source);

public class ScrapeAndProcessCommandHandler(
    ILogger<ScrapeAndProcessCommandHandler> logger,
    Context dbContext,
    IWebscraperPort webscraperPort,
    IRealtimeMessagesPort realtimeMessagesPort,
    TimeProvider timeProvider
    )
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly Context _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IWebscraperPort _webscraperPort = webscraperPort ?? throw new ArgumentNullException(nameof(webscraperPort));
    private readonly IRealtimeMessagesPort _realtimeMessagesPort = realtimeMessagesPort ?? throw new ArgumentNullException(nameof(realtimeMessagesPort));
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    public async Task Handle(ScrapeAndProcessCommand command)
    {
        _logger.LogInformation($"{command.Source}: Handle {nameof(ScrapeAndProcessCommand)}");

        var recentProjects = await _dbContext.Projects
            .Include(p => p.Tags)
            .Where(x => x.Source == command.Source && x.FirstSeenAt > _timeProvider.GetLocalNow().AddDays(-7))
            .ToArrayAsync();

        var allTags = await _dbContext.Tags.AsTracking().Include(t => t.Keywords).ToArrayAsync();

        await foreach (var project in _webscraperPort.Scrape(command.Source, recentProjects))
        {
            if (recentProjects.Any(p => p.IsSameProject(project))) continue;
            project.EvaluateAndAddTags(allTags);
            await _dbContext.Projects.AddAsync(project);
            await _dbContext.SaveChangesAsync();
            await _realtimeMessagesPort.NewProjectAdded(project);
            _logger.LogInformation($"{command.Source}: Added new project: ${project.Url}");

        }
    }
}