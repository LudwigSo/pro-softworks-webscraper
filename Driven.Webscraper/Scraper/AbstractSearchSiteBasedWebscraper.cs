using Domain.Model;

namespace Driven.Webscraper.Scraper;

public abstract class AbstractSearchSiteBasedWebscraper : AbstractCommonWebscraper
{
    public virtual int DelayPerSiteInMs => 0;
    protected virtual async Task<List<Project>> ScrapeSearchSiteOnlyNew(string url, Project? lastScrapedProject, int projectsPerPage = 20)
    {
        var lastScrapedAt = lastScrapedProject?.PostedAt ?? DateTime.Now.AddDays(-7);
        var numberOfEntries = await ScrapeNumberOfProjects(url);
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / 20);

        var projects = new List<Project>();
        for (var page = 0; page < numberOfPages; page++)
        {
            var projectUrlsFromPage = await ScrapeProjectUrlsFromSearchSite(url, page);
            var projectsFromPage = await ScrapeProjectsByUrl(projectUrlsFromPage);
            projects.AddRange(projectsFromPage);

            if (lastScrapedProject != null && projectsFromPage.Any(p => p.IsSameProject(lastScrapedProject))) break;

            var lastProject = projectsFromPage.Last();
            if (lastProject.PostedAt < lastScrapedAt) break;

            if (DelayPerSiteInMs > 0) await Task.Delay(DelayPerSiteInMs);
        }
        return projects;
    }
    protected abstract Task<int> ScrapeNumberOfProjects(string url, int retry = 0);
    protected abstract Task<string[]> ScrapeProjectUrlsFromSearchSite(string url, int page = 0, int retry = 0);
}
