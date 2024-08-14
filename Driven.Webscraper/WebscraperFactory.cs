using Application.Webscraper;
using Domain.Model;

namespace Driven.Webscraper;

public interface IWebscraper
{
    Task<List<Project>> Scrape();
}

public class WebscraperFactory : IWebscraperPort
{
    public async Task<List<Project>> Scrape(ProjectSource source)
    {
        var webscraper = CreateWebscraper(source);
        return await webscraper.Scrape();
    }

    private static IWebscraper CreateWebscraper(ProjectSource source)
    {
        return source switch
        {
            ProjectSource.Hays => new HaysWebscraper(),
            _ => throw new NotImplementedException()
        };
    }
}
