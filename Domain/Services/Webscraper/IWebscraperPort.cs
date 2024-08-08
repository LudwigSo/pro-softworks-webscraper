using Domain.Model;

namespace Domain.Services.Webscraper;

public interface IWebscraperPort
{
    Task<List<Project>> Scrape(ProjectSource source);
}
