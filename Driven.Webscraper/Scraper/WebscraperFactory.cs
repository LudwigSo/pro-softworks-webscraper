using Domain;
using Application.Ports;
using Driven.Webscraper.Proxy;

namespace Driven.Webscraper.Scraper;

public class WebscraperFactory(ILogging logger, HttpHelper httpHelper) : IWebscraperPort
{
    private readonly ILogging _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    public Task<List<Project>> Scrape(ProjectSource source, Project[]? recentProjects = null)
    {
        return source switch
        {
            ProjectSource.Hays => new HaysWebscraper(_logger, _httpHelper).Scrape(),
            ProjectSource.FreelanceDe => new FreelanceDeWebscraper(_logger, _httpHelper).ScrapeOnlyNew(recentProjects),
            ProjectSource.FreelancerMap => new FreelancerMapWebscraper(_logger, _httpHelper).ScrapeOnlyNew(),
            _ => throw new NotImplementedException()
        };
    }
}
