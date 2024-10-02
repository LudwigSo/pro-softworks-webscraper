using Domain;

namespace Application.Ports;

public interface IWebscraperPort
{
    IAsyncEnumerable<Project> Scrape(ProjectSource source, Project[]? recentProjects = null);
}
