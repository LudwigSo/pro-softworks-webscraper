using Domain.Model;

namespace Driven.Webscraper.Scraper;

public abstract class AbstractSearchSiteBasedWebscraper : AbstractCommonWebscraper
{
    protected virtual async Task<List<Project>> ScrapeSearchSiteParallel(string url, int projectsPerPage = 20)
    {
        var numberOfEntries = await ScrapeNumberOfProjects(url);
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / projectsPerPage);

        var scrapePageTasks = new List<Task<List<Project>>>();
        for (var page = 0; page < numberOfPages; page++)
        {
            scrapePageTasks.Add(ScrapeSearchPage(url, page));
        }
        Task.WaitAll([.. scrapePageTasks]);
        return scrapePageTasks.SelectMany(t => t.Result).ToList();
    }

    protected virtual async Task<List<Project>> ScrapeSearchSiteOnlyNew(string url, Project? lastScrapedProject, int projectsPerPage = 20)
    {
        if (lastScrapedProject == null) return await ScrapeSearchSiteParallel(url, projectsPerPage);

        var lastScrapedAt = lastScrapedProject.PostedAt;
        var numberOfEntries = await ScrapeNumberOfProjects(url);
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / 20);

        var projects = new List<Project>();
        for (var page = 0; page < numberOfPages; page++)
        {
            var projectUrlsFromPage = await ScrapeProjectUrlsFromSearchSite(url, page);
            var projectsFromPage = await ScrapeProjectsByUrl(projectUrlsFromPage);
            projects.AddRange(projectsFromPage);

            if (projectsFromPage.Any(p => p.IsSameProject(lastScrapedProject))) break;

            var lastProject = projectsFromPage.Last();
            if (lastProject.PostedAt < lastScrapedAt) break;
        }
        return projects;
    }

    protected virtual async Task<List<Project>> ScrapeSearchPage(string url, int page)
    {
        var projectUrlsFromPage = await ScrapeProjectUrlsFromSearchSite(url, page);
        return await ScrapeProjectsByUrl(projectUrlsFromPage);
    }

    protected abstract Task<int> ScrapeNumberOfProjects(string url, int retry = 0);
    protected abstract Task<string[]> ScrapeProjectUrlsFromSearchSite(string url, int page = 0, int retry = 0);
}
