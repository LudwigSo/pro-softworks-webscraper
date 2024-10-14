using Domain;
using Application.Ports;
using Driven.Webscraper.Proxy;
using Microsoft.Extensions.Logging;

namespace Driven.Webscraper.Scraper;

public class WebscraperFactory(
    HaysWebscraper haysWebscraper,
    FreelanceDeWebscraper freelanceDeWebscraper,
    FreelancerMapWebscraper freelancerMapWebscraper
    ) : IWebscraperPort
{
    private readonly HaysWebscraper _haysWebscraper = haysWebscraper ?? throw new ArgumentNullException(nameof(haysWebscraper));
    private readonly FreelanceDeWebscraper _freelanceDeWebscraper = freelanceDeWebscraper ?? throw new ArgumentNullException(nameof(freelanceDeWebscraper));
    private readonly FreelancerMapWebscraper _freelancerMapWebscraper = freelancerMapWebscraper ?? throw new ArgumentNullException(nameof(freelancerMapWebscraper));

    IAsyncEnumerable<Project> IWebscraperPort.Scrape(ProjectSource source, Project[]? recentProjects)
    {
        return source switch
        {
            ProjectSource.Hays => _haysWebscraper.Scrape(),
            ProjectSource.FreelanceDe => _freelanceDeWebscraper.Scrape(recentProjects),
            ProjectSource.FreelancerMap => _freelancerMapWebscraper.Scrape(recentProjects),
            _ => throw new NotImplementedException()
        };
    }
}
