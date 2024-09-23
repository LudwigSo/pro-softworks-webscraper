using Domain;

namespace Application.Ports;

public interface IWebscraperPort
{
    Task<List<Project>> Scrape(ProjectSource source, Project[]? recentProjects = null);
}
