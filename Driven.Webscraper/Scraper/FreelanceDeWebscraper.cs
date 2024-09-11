using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Domain.Model;
using Domain.Ports;
using Driven.Webscraper.Proxy;
using HtmlAgilityPack;

namespace Driven.Webscraper.Scraper;

public class FreelanceDeWebscraper(ILogger logger, HttpHelper httpHelper) : IWebscraper
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    private readonly List<string> _kategorieUrls =
    [
        "http://www.freelance.de/Projekte/K/IT-Entwicklung-Projekte/Softwareentwicklung-Softwareprogrammierung-Projekte",
    ];

    public async Task<List<Project>> Scrape()
    {
        var projects = new List<Project>();
        foreach (var kategorieUrl in _kategorieUrls)
        {
            var projectsForKategorie = await ScrapeKategorie(kategorieUrl);
            projects.AddRange(projectsForKategorie);
        }

        return projects.Distinct().ToList();
    }

    private async Task<List<Project>> ScrapeKategorie(string kategorieUrl)
    {
        var (numberOfEntries, mainPage) = await GetNumberOfProjects(kategorieUrl);
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / 20);

        //var projects = new List<Project>();
        //for(var i = 0; i < numberOfPages; i++)
        //{
        //    projects.AddRange(await ScrapeProjectSearchResults(kategorieUrl, mainPage, i));
        //}
        //return projects;
        var projectsScrapeTasks = new List<Task<List<Project>>>();
        for (var i = 0; i < numberOfPages; i++)
        {
            projectsScrapeTasks.Add(ScrapeProjectSearchResults(kategorieUrl, mainPage, i));
        }
        Task.WaitAll([.. projectsScrapeTasks]);

        return projectsScrapeTasks.SelectMany(t => t.Result).ToList();
    }

    private async Task<List<Project>> ScrapeProjectSearchResults(string kategorieUrl, HtmlDocument mainPage, int page = 0)
    {
        var projectSearchResultSite = (page == 0)
                        ? mainPage
                        : await GetProjectSearchResults(kategorieUrl, page);

        var projectUrls = GetProjectUrls(projectSearchResultSite);

        var projects = new List<Project>();
        foreach (var projectUrl in projectUrls)
        {
            try
            {
                _logger.LogInformation($"Scraping {projectUrl}");
                var project = await ScrapeProject("http://www.freelance.de" + projectUrl);
                projects.Add(project);
                _logger.LogInformation($"Success scraping project {projectUrl}");

            }
            catch
            {
                _logger.LogInformation($"Error scraping project {projectUrl}");
            }
        }
        return projects;
    }

    private async Task<HtmlDocument> GetProjectSearchResults(string kategorieUrl, int page = 0, int retry = 0)
    {
        try
        {
            var url = $"{kategorieUrl}/?_offset={(page) * 20}";
            var document = await _httpHelper.GetHtml(url);
            if (document == null) throw new InvalidOperationException("Document is null");
            return document;
        }
        catch
        {
            if (retry > 5) throw;
            return await GetProjectSearchResults(kategorieUrl, page, retry + 1);
        }
    }

    private async Task<(int, HtmlDocument)> GetNumberOfProjects(string kategorieUrl, int retry = 0)
    {
        try
        {
            var mainPage = await _httpHelper.GetHtml(kategorieUrl);
            var numberOfEntriesDiv = mainPage!.DocumentNode.SelectSingleNode("//div[@id='pagination']/p");
            var innerText = numberOfEntriesDiv.InnerText.Trim();
            innerText = RemoveNumberOfProjectsPrefix(innerText);
            innerText = RemoveNumberOfProjectsSuffix(innerText);
            return (int.Parse(innerText), mainPage);
        }
        catch
        {
            if (retry > 5) throw;
            return await GetNumberOfProjects(kategorieUrl, retry + 1);
        }
    }

    private static string RemoveNumberOfProjectsPrefix(string str) => Regex.Replace(str, @"Projekte:  \d{1,4}-\d{2,4} von ", "");
    private static string RemoveNumberOfProjectsSuffix(string str) => Regex.Replace(str, @"\D", "");

    private static string[] GetProjectUrls(HtmlDocument searchSite)
    {
        var projectUrls = searchSite.DocumentNode.SelectNodes("//a[starts-with(@id, 'project_link_')]");
        return projectUrls.Select(url => url.GetAttributeValue("href", "")).Where(s => s != "").ToArray();
    }

    private async Task<Project> ScrapeProject(string projectUrl, int retry = 0)
    {
        try
        {
            var projectSite = await _httpHelper.GetHtml(projectUrl);

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

            return project;
        }
        catch
        {
            if (retry > 5) throw;
            return await ScrapeProject(projectUrl, retry + 1);
        }
        
    }
}
