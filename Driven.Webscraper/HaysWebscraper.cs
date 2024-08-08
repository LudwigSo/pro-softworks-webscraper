using System.Text.Json;
using Domain.Model;
using HtmlAgilityPack;

namespace Driven.Webscraper;

public class HaysWebscraper : IWebscraper
{
    public async Task<List<Project>> Scrape()
    {
        var webLoader = new HtmlWeb();
        var projectSearchSite = await webLoader.LoadFromWebAsync(GetUrl());
        var numberOfEntries = GetNumberOfProjects(projectSearchSite);

        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / 20);

        var projectUrls = GetProjectUrls(projectSearchSite);
        var projects = new List<Project>();
        foreach (var projectUrl in projectUrls)
        {
            var project = ScrapeProject(projectUrl);
            projects.Add(project);
            break;
        }

        return projects;
    }

    private string GetUrl(int page = 1)
    {
        return $"https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/j/Contracting/3/p/{page}?q=&e=false&pt=false";
    }

    private int GetNumberOfProjects(HtmlDocument projectMainSite)
    {
        var numberOfEntriesDiv = projectMainSite.DocumentNode.SelectSingleNode("//div[@class='hays__search__number-of-results']");
        var numberOfEntriesText = numberOfEntriesDiv.InnerText.Replace("\n", "").Replace(" ", "").Replace("Ergebnisse", "");
        return int.Parse(numberOfEntriesText);
    }

    private List<string> GetProjectUrls(HtmlDocument searchSite)
    {
        var projectUrls = searchSite.DocumentNode.SelectNodes("//a[@class='search__result__header__a']");
        return projectUrls.Select(url => url.GetAttributeValue("href", "")).Where(s => s != "").ToList();
    }

    private Project ScrapeProject(string projectUrl)
    {
        var webLoader = new HtmlWeb();
        var projectSite = webLoader.Load(projectUrl);

        var projectScript = projectSite.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']").InnerText;

        var jsonDoc = JsonDocument.Parse(projectScript);
        var title = jsonDoc.RootElement.GetProperty("title").GetString();
        var url = jsonDoc.RootElement.GetProperty("url").GetString();

        if (title == null || url == null)
        {
            throw new InvalidOperationException($"Required fields not found in json (title: {title}, url: {url})");
        }

        jsonDoc.RootElement.TryGetProperty("identifier", out var identifier);
        jsonDoc.RootElement.TryGetProperty("description", out var description);
        jsonDoc.RootElement.TryGetProperty("jobBenefits", out var jobBenefits);

        jsonDoc.RootElement.TryGetProperty("jobLocation", out var jobLocation);
        jobLocation.TryGetProperty("@type", out var jobLocationType);
        jobLocation.TryGetProperty("address", out var jobLocationAddress);
        jobLocationAddress.TryGetProperty("addressLocality", out var jobLocationAddressLocality);
        jobLocationAddress.TryGetProperty("postalCode", out var jobLocationAddressPostalCode);
        jobLocationAddress.TryGetProperty("addressCountry", out var jobLocationAddressCountry);


        var project = new Project(
            ProjectSource.Hays,
            title,
            url,
            identifier.GetString() ?? "",
            $"{description.GetString() ?? ""} {jobBenefits.GetString() ?? ""}",
            $"{jobLocationType.GetString() ?? ""}, {jobLocationAddressCountry.GetString() ?? ""}, {jobLocationAddressPostalCode.GetString() ?? ""}, {jobLocationAddressLocality.GetString() ?? ""}"
        );

        return project;
    }
}
