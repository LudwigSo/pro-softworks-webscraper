using Domain.Model;
using Domain.Ports;
using Domain.Ports.Queries;

namespace Domain.CommandHandlers;

public sealed record ScrapeAndProcessCommand(ProjectSource Source);

public class ScrapeAndProcessCommandHandler(
    ILogger logger,
    IWebscraperPort webscraperPort,
    IProjectQueriesPort projectQueriesPort,
    ITagQueriesPort tagQueriesPort,
    IWriteContext writeContext,
    IRealtimeMessagesPort realtimeMessagesPort
    )
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IWebscraperPort _webscraperPort = webscraperPort ?? throw new ArgumentNullException(nameof(webscraperPort));
    private readonly IProjectQueriesPort _projectQueriesPort = projectQueriesPort ?? throw new ArgumentNullException(nameof(projectQueriesPort));
    private readonly ITagQueriesPort _tagQueriesPort = tagQueriesPort ?? throw new ArgumentNullException(nameof(tagQueriesPort));
    private readonly IWriteContext _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));
    private readonly IRealtimeMessagesPort _realtimeMessagesPort = realtimeMessagesPort ?? throw new ArgumentNullException(nameof(realtimeMessagesPort));


    public async Task Handle(ScrapeAndProcessCommand command)
    {
        _logger.LogInformation($"{command.Source}: Handle {nameof(ScrapeAndProcessCommand)}");


        var lastScrapedProject = await _projectQueriesPort.GetLastScrapedBySource(command.Source);
        List<Project> projects;
        try
        {
            projects = await _webscraperPort.Scrape(command.Source, lastScrapedProject);
        }
        catch (Exception e)
        {
            _logger.LogException(e, $"{command.Source}: ScrapeOnlyNew failed.");
            return;
        }

        _logger.LogInformation($"{command.Source}: {projects.Count} potential new projects found on website");

        var activeProjects = await _projectQueriesPort.GetActiveBySource(command.Source);
        await EvaluateAndAddNew(command.Source, projects, activeProjects ?? []);

    }

    private async Task EvaluateAndAddNew(ProjectSource source, List<Project> projects, Project[] activeProjects)
    {
        var newProjects = projects
            .Where(p => activeProjects.All(ap => !ap.IsSameProject(p)))
            .ToList();

        var allTags = await _tagQueriesPort.GetAllTags();
        foreach(var project in projects)
        {
            project.EvaluateAndAddTags(allTags);
        }
        _logger.LogInformation($"{source}: Tagged projects");

        await _writeContext.AddRange(newProjects);
        _logger.LogInformation($"{source}: Add {newProjects.Count} projects");

        await _writeContext.SaveChangesAsync();
        _logger.LogInformation($"{source}: Persisted new projects");

        var newProjectsWithTags = newProjects.Where(p => p.Tags.Count > 0).ToList();
        await _realtimeMessagesPort.NewProjectsAdded(newProjectsWithTags);
        _logger.LogInformation($"{source}: Published realtime {newProjectsWithTags.Count} new projects with tags");
    }
}