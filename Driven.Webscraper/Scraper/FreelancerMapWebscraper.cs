using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Domain.Model;
using Domain.Ports;
using Driven.Webscraper.Proxy;
using HtmlAgilityPack;

namespace Driven.Webscraper.Scraper;

public class FreelancerMapWebscraper(ILogger logger, HttpHelper httpHelper) : AbstractSearchSiteBasedWebscraper
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    private readonly ProjectSource _projectSource = ProjectSource.FreelancerMap;
    private readonly string _url = "http://www.freelancermap.de/projektboerse.html?categories%5B0%5D=1&projectContractTypes%5B0%5D=contracting&remoteInPercent%5B0%5D=100&remoteInPercent%5B1%5D=1&countries%5B%5D=1&countries%5B%5D=2&countries%5B%5D=3&sort=1&pagenr=1";

    public Task<List<Project>> Scrape()
    {
        return ScrapeSearchSiteParallel(_url);
    }

    public Task<List<Project>> ScrapeOnlyNew(Project? lastScrapedProject)
    {
        return ScrapeSearchSiteOnlyNew(_url, lastScrapedProject);
    }

    protected override async Task<string[]> ScrapeProjectUrlsFromSearchSite(string url, int page = 0, int retry = 0)
    {
        try
        {
            var fullUrl = $"{url}&pagenr={page +1}";
            var document = await _httpHelper.GetHtml(fullUrl);
            var projectUrlNodes = document.DocumentNode.SelectNodes("//h2/a[@class='project-title']");
            return projectUrlNodes.Select(url => "http://www.freelancermap.de" + url.GetAttributeValue("href", "")).Where(s => s != "").ToArray();
        }
        catch
        {
            if (retry > 3) throw;
            return await ScrapeProjectUrlsFromSearchSite(url, page, retry + 1);
        }
    }

    protected override async Task<int> ScrapeNumberOfProjects(string url, int retry = 0)
    {
        try
        {
            var mainPage = await _httpHelper.GetHtml(url);
            var numberOfEntriesDiv = mainPage.DocumentNode.SelectSingleNode("//div[@class='search-result-header']/div/h1");
            var innerText = numberOfEntriesDiv.InnerText.Trim();
            innerText = RemoveAnyNonNumber(innerText);
            var amountOfProjects = int.Parse(innerText);
            _logger.LogInformation($"{_projectSource}: {amountOfProjects} projects found, retry: {retry}, url {categoryUrl}");
            return amountOfProjects;
        }
        catch
        {
            if (retry > 3) throw;
            return await ScrapeNumberOfProjects(url, retry + 1);
        }
    }

    private static string RemoveAnyNonNumber(string str) => Regex.Replace(str, @"\D", "");

    protected override async Task<Project?> ScrapeProject(string projectUrl, int retry = 0)
    {
        try
        {
            _logger.LogDebug($"{_projectSource}: Start to scrape project, retry: {retry}, url {projectUrl}");
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

            _logger.LogInformation($"{_projectSource}: Succeeded scraping project, retry: {retry}, url {projectUrl}");
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
