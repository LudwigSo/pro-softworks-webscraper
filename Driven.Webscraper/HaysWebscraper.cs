using System.Text.Json;
using Domain.Model;
using HtmlAgilityPack;

namespace Driven.Webscraper;

public class HaysWebscraper : IWebscraper
{
    private readonly List<string> _haysSpezialisierungsUrls = new List<string>
    {
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Softwareentwickler/8D391CE4-6175-469A-936E-CA694A52E8AC/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Softwarearchitekt/4002EE17-5727-44F8-A5DF-CB51F4D64097/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Softwaretester/5E0C2C71-2260-4C1A-A4CD-62E7FABF0D63/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Architekt/7FA1A118-D0EA-445C-AFA9-BFA05F3A36BF/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Applikationsingenieur/E9BF9DE1-89BA-42FA-8DB3-70C4E43D1443/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Cloud-Engineer/4F70CA09-E2E8-4AAC-9B54-5F900246F844/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/IT-Berater/B0C4E890-8BCF-4D2A-A5FB-63B26B5C4150/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Systemingenieur/0793F3E3-0C39-40DE-8B65-9962666F635E/j/Contracting/3/p/1?q=&e=false&pt=false"
    };

    public async Task<List<Project>> Scrape()
    {
        var projects = new List<Project>();
        foreach (var spezialisierungUrl in _haysSpezialisierungsUrls)
        {
            var projectsForSpezialisierung = await ScrapeSpezialisierung(spezialisierungUrl);
            projects.AddRange(projectsForSpezialisierung);
        }

        return projects;
    }

    private async Task<List<Project>> ScrapeSpezialisierung(string spezialisierungUrl)
    {
        var webLoader = new HtmlWeb();
        var mainPage = await webLoader.LoadFromWebAsync(spezialisierungUrl);
        var numberOfEntries = GetNumberOfProjects(mainPage);
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / 20);
        var projectSearchResultSites = new List<HtmlDocument> { mainPage };

        for (var i = 2; i <= numberOfPages; i++)
        {
            var projectSearchResultSite = await webLoader.LoadFromWebAsync(GetCompleteUrl(spezialisierungUrl, i));
            projectSearchResultSites.Add(projectSearchResultSite);
        }

        var projectUrls = projectSearchResultSites.SelectMany(GetProjectUrls).ToList();
        var projects = new List<Project>();
        foreach (var projectUrl in projectUrls)
        {
            var project = ScrapeProject(projectUrl);
            projects.Add(project);
        }

        return projects;
    }

    private string GetCompleteUrl(string spezialisierungsUrl, int page = 1)
    {
        return $"{spezialisierungsUrl}/j/Contracting/3/p/{page}?q=&e=false&pt=false";
    }

    private string GetUrl(int page = 1)
    {
        return $"https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/j/Contracting/3/p/{page}?q=&e=false&pt=false";
    }

    private int GetNumberOfProjects(HtmlDocument projectMainSite)
    {
        var numberOfEntriesDiv = projectMainSite.DocumentNode.SelectSingleNode("//div[@class='hays__search__number-of-results']");
        var numberOfEntriesText = numberOfEntriesDiv.InnerText.Replace("\n", "").Replace(" ", "").Replace("Ergebnisse", "").Replace("Ergebnis", "");
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
