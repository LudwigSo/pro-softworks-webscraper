using Domain.Model;
using Domain.Ports;
using Driven.Webscraper.Proxy;

namespace Driven.Webscraper.Scraper;

public interface IWebscraper
{
    Task<List<Project>> Scrape();
}

public class WebscraperFactory(ILogger logger, HttpHelper httpHelper) : IWebscraperPort
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    public Task<List<Project>> Scrape(ProjectSource source)
    {
        return source switch
        {
            ProjectSource.Hays => new HaysWebscraper(_logger).Scrape(),
            ProjectSource.FreelanceDe => new FreelanceDeWebscraper(_logger, _httpHelper).Scrape(),
            _ => throw new NotImplementedException()
        };
    }

    public bool ScrapeOnlyNewSupported(ProjectSource source)
    {
        return source switch
        {
            ProjectSource.FreelanceDe => true,
            _ => false
        };
    }

    public Task<List<Project>> ScrapeOnlyNew(ProjectSource source, Project lastScrapedProject)
    {
        return source switch
        {
            ProjectSource.FreelanceDe => new FreelanceDeWebscraper(_logger, _httpHelper).ScrapeOnlyNew(lastScrapedProject),
            _ => throw new NotImplementedException()
        };
    }
}
