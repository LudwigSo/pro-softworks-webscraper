using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;
using Domain.Model;
using Domain.Ports;
using HtmlAgilityPack;

namespace Driven.Webscraper;

public class FreelanceDeWebscraper(ILogger logger) : IWebscraper
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    private readonly List<string> _kategorieUrls =
    [
        "https://www.freelance.de/Projekte/K/IT-Entwicklung-Projekte/Softwareentwicklung-Softwareprogrammierung-Projekte",
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
        var webLoader = new HtmlWeb();
        var mainPage = await webLoader.LoadFromWebAsync(kategorieUrl);
        var numberOfEntries = GetNumberOfProjects(mainPage);
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / 20);
        var projectSearchResultSites = new List<HtmlDocument> { mainPage };

        for (var i = 2; i <= numberOfPages; i++)
        {
            var projectSearchResultSite = await webLoader.LoadFromWebAsync(GetCompleteUrl(kategorieUrl, i));
            projectSearchResultSites.Add(projectSearchResultSite);
        }

        var projectUrls = projectSearchResultSites.SelectMany(GetProjectUrls).ToList();
        var projects = new List<Project>();
        foreach (var projectUrl in projectUrls)
        {
            try
            {
                var project = await ScrapeProject("https://www.freelance.de" + projectUrl);
                projects.Add(project);
            }
            catch (Exception e)
            {
                _logger.LogException(e, $"Error scraping project {projectUrl}");
            }
        }
        return projects;
    }

    private static string GetCompleteUrl(string kategorieUrl, int page = 1)
    {
        return $"{kategorieUrl}/?_offset={(page-1)*20}";
    }

    private static int GetNumberOfProjects(HtmlDocument mainPage)
    {
        var numberOfEntriesDiv = mainPage.DocumentNode.SelectSingleNode("//div[@id='pagination']/p");
        var innerText = numberOfEntriesDiv.InnerText.Trim();
        innerText = RemoveNumberOfProjectsPrefix(innerText);
        innerText = RemoveNumberOfProjectsSuffix(innerText);
        return int.Parse(innerText);
    }

    private static string RemoveNumberOfProjectsPrefix(string str) => Regex.Replace(str, @"Projekte:  \d{1,4}-\d{2,4} von ", "");
    private static string RemoveNumberOfProjectsSuffix(string str) => Regex.Replace(str, @"\D", "");

    private static string[] GetProjectUrls(HtmlDocument searchSite)
    {
        var projectUrls = searchSite.DocumentNode.SelectNodes("//a[starts-with(@id, 'project_link_')]");
        return projectUrls.Select(url => url.GetAttributeValue("href", "")).Where(s => s != "").ToArray();
    }

    private async Task<Project> ScrapeProject(string projectUrl)
    {
        var webLoader = new HtmlWeb();
        var projectSite = await webLoader.LoadFromWebAsync(projectUrl);
        _logger.LogInformation($"Scraping {projectUrl}");

        var title = projectSite.DocumentNode.SelectSingleNode("//h1").InnerText;
        var identifier = projectSite.DocumentNode.SelectSingleNode("//i[@data-original-title='Referenz-Nummer']/parent::li")?.InnerText?.Trim();
        var jobLocation = projectSite.DocumentNode.SelectSingleNode("//i[@data-original-title='Projektort']/parent::li")?.InnerText?.Trim();
        var scriptNodes = projectSite.DocumentNode.SelectNodes("//script[@type='application/ld+json']");
        var description = string.Empty;
        var descriptionNode = scriptNodes.Where(n => n.InnerText.StartsWith("{\"datePosted")).SingleOrDefault();
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
            ProjectSource.FreelanceDe,
            title,
            projectUrl,
            identifier,
            description,
            jobLocation
        );

        return project;
    }
}
