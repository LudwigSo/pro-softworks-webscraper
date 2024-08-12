using Domain.Model;

namespace Application.Webscraper;

public interface IWebscraperPort
{
    Task<List<Project>> Scrape(ProjectSource source);
}
