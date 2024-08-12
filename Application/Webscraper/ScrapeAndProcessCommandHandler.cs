using Application.Ports;
using Domain.Model;

namespace Application.Webscraper;

public sealed record ScrapeAndProcessCommand(ProjectSource Source);

public class ScrapeAndProcessCommandHandler(IWebscraperPort webscraperPort, IReadContext readContext, IWriteContext writeContext)
{
    private readonly IWebscraperPort _webscraperPort = webscraperPort ?? throw new ArgumentNullException(nameof(webscraperPort));
    private readonly IReadContext _readContext = readContext ?? throw new ArgumentNullException(nameof(readContext));
    private readonly IWriteContext _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));


    public async Task Handle(ScrapeAndProcessCommand command)
    {
        await ScrapeAndProcess(command.Source);
    }

    private async Task ScrapeAndProcess(ProjectSource source)
    {
        var projects = await _webscraperPort.Scrape(source);
        Console.WriteLine($"{source}: {projects.Count} projects found on website");

        var activeProjects = await GetActiveBySource(source);

        var removedProjects = activeProjects.Where(p => projects.All(ap => !ap.IsSameProject(p)));
        foreach (var removedProject in removedProjects)
        {
            removedProject.MarkAsRemoved();
        }
        Console.WriteLine($"{source}: Remove {removedProjects.Count()} projects");

        var newProjects = projects.Where(p => activeProjects.All(ap => !ap.IsSameProject(p)));
        await _writeContext.AddRange(newProjects);
        Console.WriteLine($"{source}: Add {newProjects.Count()} projects");

        await _writeContext.SaveChangesAsync();
        Console.WriteLine($"{source}: Persisted changes");
    }

    internal async Task<List<Project>> GetActiveBySource(ProjectSource source)
    {
        var query = _readContext.Projects.Where(p => p.Source == source && p.RemovedAt == null);
        return await _readContext.ToListAsync(query);
    }
}