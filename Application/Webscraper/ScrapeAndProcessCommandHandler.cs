using Application.Ports;
using Domain.Model;
using Domain.Ports;

namespace Application.Webscraper;

public sealed record ScrapeAndProcessCommand(ProjectSource Source);

public class ScrapeAndProcessCommandHandler(
    IWebscraperPort webscraperPort, 
    IReadContext readContext, 
    IWriteContext writeContext,
    ILogger logger
    )
{
    private readonly IWebscraperPort _webscraperPort = webscraperPort ?? throw new ArgumentNullException(nameof(webscraperPort));
    private readonly IReadContext _readContext = readContext ?? throw new ArgumentNullException(nameof(readContext));
    private readonly IWriteContext _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));


    public async Task Handle(ScrapeAndProcessCommand command)
    {
        await ScrapeAndProcess(command.Source);
    }

    private async Task ScrapeAndProcess(ProjectSource source)
    {
        var projects = await _webscraperPort.Scrape(source);
        _logger.LogInformation($"{source}: {projects.Count} projects found on website");

        var activeProjects = await GetActiveBySource(source);

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
    }

    private async Task<List<Project>> GetActiveBySource(ProjectSource source)
    {
        var query = _readContext.Projects.Where(p => p.Source == source && p.IsActive);
        return await _readContext.ToListAsync(query);
    }
}