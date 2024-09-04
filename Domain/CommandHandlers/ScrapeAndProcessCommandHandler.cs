using Domain.Model;
using Domain.Ports;

namespace Domain.CommandHandlers;

public sealed record ScrapeAndProcessCommand(ProjectSource Source);

public class ScrapeAndProcessCommandHandler(
    IWebscraperPort webscraperPort,
    IProjectQueriesPort projectQueriesPort,
    IWriteContext writeContext,
    IRealtimeMessagesPort realtimeMessagesPort,
    ILogger logger
    )
{
    private readonly IWebscraperPort _webscraperPort = webscraperPort ?? throw new ArgumentNullException(nameof(webscraperPort));
    private readonly IProjectQueriesPort _projectQueriesPort = projectQueriesPort ?? throw new ArgumentNullException(nameof(projectQueriesPort));
    private readonly IWriteContext _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));
    private readonly IRealtimeMessagesPort _realtimeMessagesPort = realtimeMessagesPort ?? throw new ArgumentNullException(nameof(realtimeMessagesPort));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));


    public Task Handle(ScrapeAndProcessCommand command)
    {
        return ScrapeAndProcess(command.Source);
    }

    private async Task ScrapeAndProcess(ProjectSource source)
    {
        var projects = await _webscraperPort.Scrape(source);
        _logger.LogInformation($"{source}: {projects.Count} projects found on website");

        var activeProjects = await _projectQueriesPort.GetActiveBySource(source);

        var removedProjects = activeProjects
            .Where(p => projects.All(ap => !ap.IsSameProject(p)))
            .ToList();

        foreach (var removedProject in removedProjects)
        {
            removedProject.MarkAsRemoved();
        }
        _logger.LogInformation($"{source}: Remove {removedProjects.Count} projects");

        var newProjects = projects
            .Where(p => activeProjects.All(ap => !ap.IsSameProject(p)))
            .ToList();
        await _writeContext.AddRange(newProjects);
        _logger.LogInformation($"{source}: Add {newProjects.Count} projects");

        await _writeContext.SaveChangesAsync();
        _logger.LogInformation($"{source}: Persisted changes");

        await _realtimeMessagesPort.NewProjectsAdded(newProjects);
        await _realtimeMessagesPort.ProjectsRemoved(removedProjects);
        _logger.LogInformation($"{source}: Project changes (added/removed) published");
    }
}