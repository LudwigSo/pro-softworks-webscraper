using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Domain.Model;
using Domain.Ports;
using Driven.Webscraper.Proxy;
using HtmlAgilityPack;

namespace Driven.Webscraper.Scraper;

public class FreelancerMapWebscraper(ILogger logger, HttpHelper httpHelper) : IWebscraper
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    private readonly string _url = "http://www.freelancermap.de/projektboerse.html?categories%5B0%5D=1&projectContractTypes%5B0%5D=contracting&remoteInPercent%5B0%5D=100&remoteInPercent%5B1%5D=1&countries%5B%5D=1&countries%5B%5D=2&countries%5B%5D=3&sort=1&pagenr=1";


    public Task<List<Project>> Scrape()
    {
        return Scrape(null);
    }

    public Task<List<Project>> ScrapeOnlyNew(Project? LastScrapedProject)
    {
        return Scrape(LastScrapedProject?.PostedAt);
    }

    private async Task<List<Project>> Scrape(DateTime? lastScrapedAt)
    {
        var (numberOfEntries, mainPage) = await GetNumberOfProjects();
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / 20);

        var projects = new List<Project>();
        for (var page = 0; page < numberOfPages; page++)
        {
            var projectsFromPage = await ScrapeProjectSearchResults((mainPage, ExtractProjectUrls(mainPage)), page);
            projects.AddRange(projectsFromPage);
            if (lastScrapedAt.HasValue)
            {
                var lastProject = projectsFromPage.Last();
                if (lastProject.PostedAt < lastScrapedAt) break;
            }
        }
        return projects;
    }

    private async Task<List<Project>> ScrapeProjectSearchResults((HtmlDocument Html, string[] ProjectUrls) mainPage, int page = 0)
    {
        var searchResult = (page == 0)
                        ? mainPage
                        : await GetProjectSearchResults(page);

        var projectScrapeTasks = new List<Task<Project>>();
        foreach (var projectUrl in searchResult.ProjectUrls)
        {
            projectScrapeTasks.Add(ScrapeProject("http://www.freelancermap.de" + projectUrl));
        }
        Task.WaitAll([.. projectScrapeTasks]);

        return projectScrapeTasks.Select(t => t.Result).ToList();
    }

    private async Task<(HtmlDocument Html, string[] ProjectUrls)> GetProjectSearchResults(int page = 0, int retry = 0)
    {
        try
        {
            var url = $"{_url}&pagenr={page +1}";
            var document = await _httpHelper.GetHtml(url);
            return (document, ExtractProjectUrls(document));
        }
        catch
        {
            if (retry > 5) throw;
            return await GetProjectSearchResults(page, retry + 1);
        }
    }

    private static string[] ExtractProjectUrls(HtmlDocument document)
    {
        var projectUrlNodes = document.DocumentNode.SelectNodes("//h2/a[@class='project-title']");
        return projectUrlNodes.Select(url => url.GetAttributeValue("href", "")).Where(s => s != "").ToArray();
    }

    private async Task<(int, HtmlDocument)> GetNumberOfProjects(int retry = 0)
    {
        try
        {
            var mainPage = await _httpHelper.GetHtml(_url);
            var numberOfEntriesDiv = mainPage.DocumentNode.SelectSingleNode("//div[@class='search-result-header']/div/h1");
            var innerText = numberOfEntriesDiv.InnerText.Trim();
            innerText = RemoveAnyNonNumber(innerText);
            return (int.Parse(innerText), mainPage);
        }
        catch
        {
            if (retry > 5) throw;
            return await GetNumberOfProjects(retry + 1);
        }
    }

    private static string RemoveAnyNonNumber(string str) => Regex.Replace(str, @"\D", "");


    private async Task<Project> ScrapeProject(string projectUrl, int retry = 0)
    {
        try
        {
            _logger.LogInformation($"Scraping project, retry: {retry}, url {projectUrl}");
            var projectSite = await _httpHelper.GetHtml(projectUrl);

            var title = projectSite.DocumentNode.SelectSingleNode("//h1").InnerText?.Trim();
            if (string.IsNullOrEmpty(title)) throw new InvalidOperationException($"Title is empty, url: {projectUrl}");

            var identifier = projectSite.DocumentNode.SelectSingleNode("//dl/dt[text()='Projekt-ID:']/following-sibling::dd[1]")?.InnerText?.Trim();
            var jobLocation = projectSite.DocumentNode.SelectSingleNode("//span[@class='address']")?.InnerText?.Trim();

            var plannedStartString = projectSite.DocumentNode.SelectSingleNode("//dl/dt[text()='Start']/following-sibling::dd[1]")?.InnerText?.Trim();
            DateTime? plannedStart;
            try 
            {
                plannedStart = plannedStartString == null ? null : DateTime.ParseExact(plannedStartString, "MM.yyyy", new CultureInfo("de-DE"), DateTimeStyles.AssumeLocal);
                if (plannedStart.HasValue)
                {
                    plannedStart.Value.AddDays((double)-plannedStart.Value.Day + 1);
                    plannedStart.Value.AddHours((double)-plannedStart.Value.Hour + 1);
                    plannedStart.Value.AddMinutes((double)-plannedStart.Value.Minute + 1);
                    plannedStart.Value.AddSeconds((double)-plannedStart.Value.Second + 1);
                }
            }
            catch
            {
                // most likely a text with "ab sofort" or "asap"
                plannedStart = DateTime.Now;
            }

            var postedAtString = projectSite.DocumentNode.SelectSingleNode("//dl/dt[text()='Eingestellt']/following-sibling::dd[1]")?.InnerText?.Trim();
            DateTime? postedAt = postedAtString == null ? null : DateTime.ParseExact(postedAtString, "dd.MM.yyyy", new CultureInfo("de-DE"), DateTimeStyles.AssumeLocal);

            var description = projectSite.DocumentNode.SelectSingleNode("//div[@class='description']/div[@class='projectcontent']/div[@class='content']").InnerText.Trim();
            if (string.IsNullOrEmpty(description)) throw new InvalidOperationException($"Description is empty, url: {projectUrl}");


            var project = new Project(
                source: ProjectSource.FreelancerMap,
                title: title,
                url: projectUrl,
                projectIdentifier: identifier,
                description: description,
                jobLocation: jobLocation,
                plannedStart: plannedStart,
                postedAt: postedAt
            );

            _logger.LogInformation($"Success scraping project, retry: {retry}, url {projectUrl}");
            return project;
        }
        catch
        {
            _logger.LogInformation($"Error scraping project, retry: {retry}, url {projectUrl}");
            if (retry > 5) throw;
            return await ScrapeProject(projectUrl, retry + 1);
        }

    }
}
