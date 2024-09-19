using Domain.Model;
using Domain.Ports;
using Driven.Webscraper.Proxy;

namespace Driven.Webscraper.Scraper;

public class WebscraperFactory(ILogger logger, HttpHelper httpHelper) : IWebscraperPort
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    public Task<List<Project>> Scrape(ProjectSource source, Project? lastScrapedProject)
    {
        return source switch
        {
            ProjectSource.Hays => new HaysWebscraper(_logger, _httpHelper).Scrape(),
            ProjectSource.FreelanceDe => new FreelanceDeWebscraper(_logger, _httpHelper).ScrapeOnlyNew(lastScrapedProject),
            ProjectSource.FreelancerMap => new FreelancerMapWebscraper(_logger, _httpHelper).ScrapeOnlyNew(lastScrapedProject),
            _ => throw new NotImplementedException()
        };
    }
}
