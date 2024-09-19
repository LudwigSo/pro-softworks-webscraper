using System.Globalization;
using System.Text.Json;
using Domain;
using Application.Ports;
using Driven.Webscraper.Proxy;

namespace Driven.Webscraper.Scraper;

public class FreelanceDeWebscraper(ILogger logger, HttpHelper httpHelper) : AbstractCommonWebscraper
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    private readonly ProjectSource _projectSource = ProjectSource.FreelanceDe;
    private readonly List<string> _categoryUrls =
    [
        "http://www.freelance.de/Projekte/K/IT-Entwicklung-Projekte/Softwareentwicklung-Softwareprogrammierung-Projekte",
        "http://www.freelance.de/Projekte/K/IT-Entwicklung-Projekte/Softwarearchitektur-Softwareanalyse-Projekte",
        "http://www.freelance.de/Projekte/K/IT-Entwicklung-Projekte/User-Interface-User-Experience-Projekte",
        "http://www.freelance.de/Projekte/K/IT-Entwicklung-Projekte/Web-Projekte"
    ];

    public async Task<List<Project>> ScrapeOnlyNew(Project[]? recentProjects)
    {
        var projects = new List<Project>();
        foreach (var categoryUrl in _categoryUrls)
        {
            var projectUrlsFromPage = await ScrapeProjectUrlsFromSearchSite(categoryUrl);
            var projectsFromPage = await ScrapeProjectsByUrl(projectUrlsFromPage, recentProjects: recentProjects, delayPerProjectInMs: 2000);
            projects.AddRange(projectsFromPage);
        }

        return projects.Distinct().ToList();
    }

    protected async Task<string[]> ScrapeProjectUrlsFromSearchSite(string categoryUrl, int page = 0, int retry = 0)
    {
        try
        {
            var url = $"{categoryUrl}/?_offset={(page) * 20}";
            var document = await _httpHelper.GetHtml(url);
            var projectUrlNodes = document.DocumentNode.SelectNodes("//a[starts-with(@id, 'project_link_')]");
            return projectUrlNodes.Select(url => "http://www.freelance.de" + url.GetAttributeValue("href", "")).Where(s => s != "").ToArray();
        }
        catch
        {
            if (retry > 3) throw;
            return await ScrapeProjectUrlsFromSearchSite(categoryUrl, page, retry + 1);
        }
    }

    protected override async Task<Project?> ScrapeProject(string projectUrl, int retry = 0)
    {
        try
        {
            _logger.LogDebug($"{_projectSource}: Start to scrape project, retry: {retry}, url {projectUrl}");
            var projectSite = await _httpHelper.GetHtml(projectUrl, retryDelayInMs: 2000);

            var title = projectSite!.DocumentNode.SelectSingleNode("//h1").InnerText;
            var identifier = projectSite.DocumentNode.SelectSingleNode("//i[@data-original-title='Referenz-Nummer']/parent::li")?.InnerText?.Trim();
            var jobLocation = projectSite.DocumentNode.SelectSingleNode("//i[@data-original-title='Projektort']/parent::li")?.InnerText?.Trim();

            var plannedStartString = projectSite.DocumentNode.SelectSingleNode("//i[@data-original-title='Geplanter Start']/parent::li")?.InnerText?.Trim();
            DateTime? plannedStart = plannedStartString == null ? null : DateTime.ParseExact(plannedStartString, "MMMM yyyy", new CultureInfo("de-DE"), DateTimeStyles.AssumeLocal);
            if (plannedStart.HasValue)
            {
                plannedStart.Value.AddDays((double)-plannedStart.Value.Day + 1);
                plannedStart.Value.AddHours((double)-plannedStart.Value.Hour + 1);
                plannedStart.Value.AddMinutes((double)-plannedStart.Value.Minute + 1);
                plannedStart.Value.AddSeconds((double)-plannedStart.Value.Second + 1);
            }

            var postedAtString = projectSite.DocumentNode.SelectSingleNode("//i[@data-original-title='Letztes Update']/parent::li")?.InnerText?.Trim();
            DateTime? postedAt = postedAtString == null ? null : DateTime.ParseExact(postedAtString, "dd.MM.yyyy", new CultureInfo("de-DE"), DateTimeStyles.AssumeLocal);


            var scriptNodes = projectSite.DocumentNode.SelectNodes("//script[@type='application/ld+json']");
            var description = string.Empty;
            var descriptionNode = scriptNodes.SingleOrDefault(n => n.InnerText.StartsWith("{\"datePosted"));
            if (descriptionNode != null)
            {
                var descriptionJson = JsonDocument.Parse(descriptionNode.InnerText);
                description = descriptionJson.RootElement.GetProperty("description").GetString();
            }
            else
            {
                description = projectSite.DocumentNode.SelectSingleNode("//div[@class='panel-body highlight-text']").InnerText.Trim();
            }

            if (description == string.Empty) throw new InvalidOperationException($"Description is empty, url: {projectUrl}");


            var project = new Project(
                source: ProjectSource.FreelanceDe,
                title: title,
                url: projectUrl,
                projectIdentifier: identifier,
                description: description,
                jobLocation: jobLocation,
                plannedStart: plannedStart,
                postedAt: postedAt
            );

            _logger.LogInformation($"Succeeded scraping project ({retry}): {project.ToLogMessage()}");
            return project;
        }
        catch
        {
            _logger.LogDebug($"{_projectSource}: Failed scraping project, retry: {retry}, url {projectUrl}");
            if (retry > 2) return null;
            return await ScrapeProject(projectUrl, retry + 1);
        }
        
    }
}
