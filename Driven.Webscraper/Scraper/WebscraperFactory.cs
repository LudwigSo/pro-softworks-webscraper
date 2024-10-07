using Domain;
using Application.Ports;
using Driven.Webscraper.Proxy;
using Microsoft.Extensions.Logging;

namespace Driven.Webscraper.Scraper;

public class WebscraperFactory(ILogger logger, HttpHelper httpHelper) : IWebscraperPort
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    IAsyncEnumerable<Project> IWebscraperPort.Scrape(ProjectSource source, Project[]? recentProjects)
    {
        return source switch
        {
            ProjectSource.Hays => new HaysWebscraper(_logger, _httpHelper).Scrape(),
            ProjectSource.FreelanceDe => new FreelanceDeWebscraper(_logger, _httpHelper).Scrape(recentProjects),
            ProjectSource.FreelancerMap => new FreelancerMapWebscraper(_logger, _httpHelper).Scrape(recentProjects),
            _ => throw new NotImplementedException()
        };
    }
}
