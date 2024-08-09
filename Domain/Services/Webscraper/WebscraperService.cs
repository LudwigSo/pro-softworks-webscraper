using Domain.Model;

namespace Domain.Services.Webscraper;

public class WebscraperService(IWebscraperPort port, IProjectQueries projectQueries, ICrudUnitOfWork unitOfWork)
{
    private readonly IWebscraperPort _webscraperPort = port ?? throw new ArgumentNullException(nameof(port));
    private readonly IProjectQueries _projectQueries = projectQueries ?? throw new ArgumentNullException(nameof(projectQueries));
    private readonly ICrudUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task ScrapeAndProcess()
    {
        await ScrapeAndProcess(ProjectSource.Hays);
    }

    public async Task ScrapeAndProcess(ProjectSource source)
    {
        var projects = await _webscraperPort.Scrape(source);
        var activeProjects = await _projectQueries.GetActiveBySource(source);
        Console.WriteLine($"{source}: {activeProjects.Count()} active projects found");

        var removedProjects = activeProjects.Where(p => projects.All(ap => !ap.IsSameProject(p)));
        foreach (var removedProject in removedProjects)
        {
            removedProject.MarkAsRemoved();
        }
        Console.WriteLine($"{source}: Remove {removedProjects.Count()} projects");

        var newProjects = projects.Where(p => activeProjects.All(ap => !ap.IsSameProject(p)));
        await _unitOfWork.AddRange(newProjects);
        Console.WriteLine($"{source}: Add {newProjects.Count()} projects");

        await _unitOfWork.SaveChangesAsync();
        Console.WriteLine($"{source}: Persisted changes");
    }
}