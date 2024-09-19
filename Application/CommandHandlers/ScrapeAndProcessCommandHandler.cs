using Application.Ports;
using Domain;
using Driven.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Application.CommandHandlers;

public sealed record ScrapeAndProcessCommand(ProjectSource Source);

public class ScrapeAndProcessCommandHandler(
    ILogger logger,
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
            .Where(x => x.Source == command.Source && x.PostedAt.HasValue && x.FirstSeenAt > _timeProvider.GetLocalNow().AddDays(-7))
            .ToArrayAsync();

        List<Project> projects;
        try
        {
            projects = await _webscraperPort.Scrape(command.Source, recentProjects);
        }
        catch (Exception e)
        {
            _logger.LogException(e, $"{command.Source}: Scrape failed.");
            return;
        }

        _logger.LogInformation($"{command.Source}: {projects.Count} potential new projects found on website");


        var newProjects = projects
            .Where(p => recentProjects.All(ap => !ap.IsSameProject(p)))
            .ToList();

        if (newProjects == null || newProjects.Count == 0)
        {
            _logger.LogInformation($"{command.Source}: No new projects found");
            return;
        }

        var allTags = await _dbContext.Tags.Include(t => t.Keywords).ToArrayAsync();
        foreach (var project in projects)
        {
            project.EvaluateAndAddTags(allTags);
        }
        _logger.LogInformation($"{command.Source}: Tagged projects");

        await _dbContext.Projects.AddRangeAsync(newProjects);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"{command.Source}: Added {newProjects.Count} projects");

        var newProjectsWithTags = newProjects.Where(p => p.Tags.Count > 0).ToList();
        await _realtimeMessagesPort.NewProjectsAdded(newProjectsWithTags);
        _logger.LogInformation($"{command.Source}: Published realtime {newProjectsWithTags.Count} new projects with tags");

    }
}