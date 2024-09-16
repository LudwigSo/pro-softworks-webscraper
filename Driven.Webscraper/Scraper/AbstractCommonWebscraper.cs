using Domain.Model;

namespace Driven.Webscraper.Scraper;

public abstract class AbstractCommonWebscraper
{
    protected async Task<List<Project>> ScrapeProjectsByUrl(string[] projectUrls)
    {
        var projectScrapeTasks = new List<Task<Project?>>();
        foreach (var projectUrl in projectUrls)
        {
            projectScrapeTasks.Add(ScrapeProject(projectUrl));
        }

        List<Project> projects = new();
        foreach (var projectScrapeTask in projectScrapeTasks)
        {
            var project = await projectScrapeTask;
            if (project == null) continue;
            projects.Add(project);
        }

        return projects;
    }

    protected abstract Task<Project?> ScrapeProject(string projectUrl, int retry = 0);

}
