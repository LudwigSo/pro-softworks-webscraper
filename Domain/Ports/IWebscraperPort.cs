using Domain.Model;

namespace Domain.Ports;

public interface IWebscraperPort
{
    Task<List<Project>> Scrape(ProjectSource source, Project? lastScrapedProject);
}
