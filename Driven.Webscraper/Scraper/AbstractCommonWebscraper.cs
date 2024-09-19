using Domain;

namespace Driven.Webscraper.Scraper;

public abstract class AbstractCommonWebscraper
{
    protected async Task<List<Project>> ScrapeProjectsByUrlParallel(string[] projectUrls)
    {
        List<Project> projects = new();
        var projectScrapeTasks = new List<Task<Project?>>();

        foreach (var projectUrl in projectUrls)
        {
            var projectScrapeTask = ScrapeProject(projectUrl);
            projectScrapeTasks.Add(projectScrapeTask);
        }

        foreach (var projectScrapeTask in projectScrapeTasks)
        {
            var project = await projectScrapeTask;
            if (project == null) continue;
            projects.Add(project);
        }

        return projects;
    }

    protected async Task<List<Project>> ScrapeProjectsByUrl(string[] projectUrls, List<Project>? recentFreelanceDeProjects = null, int delayPerProjectInMs = 0)
    {
        List<Project> projects = new();

        foreach (var projectUrl in projectUrls)
        {
            var project = await ScrapeProject(projectUrl);
            if (project == null) continue;

            projects.Add(project);

            if (recentFreelanceDeProjects != null && recentFreelanceDeProjects.Any(p => p.IsSameProject(project))) break;
            if (delayPerProjectInMs > 0) await Task.Delay(delayPerProjectInMs);
        }


        return projects;
    }

    protected abstract Task<Project?> ScrapeProject(string projectUrl, int retry = 0);

}
