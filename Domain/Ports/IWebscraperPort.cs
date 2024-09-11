using Domain.Model;

namespace Domain.Ports;

public interface IWebscraperPort
{
    Task<List<Project>> Scrape(ProjectSource source);
    bool ScrapeOnlyNewSupported(ProjectSource source);
    Task<List<Project>> ScrapeOnlyNew(ProjectSource source, Project lastScrapedProject);
}
