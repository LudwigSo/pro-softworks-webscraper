using Domain.Model;
using Domain.Ports;

namespace Driven.Webscraper.Scraper;

public interface IWebscraper
{
    Task<List<Project>> Scrape();
}

public class WebscraperFactory(ILogger logger) : IWebscraperPort
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    public async Task<List<Project>> Scrape(ProjectSource source)
    {
        var webscraper = CreateWebscraper(source);
        return await webscraper.Scrape();
    }

    private IWebscraper CreateWebscraper(ProjectSource source)
    {
        return source switch
        {
            ProjectSource.Hays => new HaysWebscraper(_logger),
            ProjectSource.FreelanceDe => new FreelanceDeWebscraper(_logger),
            _ => throw new NotImplementedException()
        };
    }
}
