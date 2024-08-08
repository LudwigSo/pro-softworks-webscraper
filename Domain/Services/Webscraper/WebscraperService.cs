using Domain.Model;

namespace Domain.Services.Webscraper;

public class WebscraperService(IWebscraperPort port, IProjectQueries projectQueries, ICrudUnitOfWork unitOfWork)
{
    private readonly IWebscraperPort _webscraperPort = port ?? throw new ArgumentNullException(nameof(port));
    private readonly IProjectQueries _projectQueries = projectQueries ?? throw new ArgumentNullException(nameof(projectQueries));
    private readonly ICrudUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task ScrapeAndProcess()
    {
        var projects = await _webscraperPort.Scrape(ProjectSource.Hays);
        var activeProjects = await _projectQueries.GetActiveBySource(ProjectSource.Hays);

        var removedProjects = activeProjects.Where(p => projects.All(ap => !ap.IsSameProject(p)));
        foreach (var removedProject in removedProjects)
        {
            removedProject.MarkAsRemoved();
        }

        var newProjects = projects.Where(p => activeProjects.All(ap => !ap.IsSameProject(p)));
        await _unitOfWork.AddRange(newProjects);

        await _unitOfWork.SaveChangesAsync();
    }
}