using System.Text.Json;
using Domain.Model;
using Domain.Ports;
using Driven.Webscraper.Proxy;
using HtmlAgilityPack;

namespace Driven.Webscraper.Scraper;

public class HaysWebscraper(ILogger logger, HttpHelper httpHelper) : IWebscraper
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    private readonly List<string> _haysSpezialisierungsUrls =
    [
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Softwareentwickler/8D391CE4-6175-469A-936E-CA694A52E8AC/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Softwarearchitekt/4002EE17-5727-44F8-A5DF-CB51F4D64097/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Softwaretester/5E0C2C71-2260-4C1A-A4CD-62E7FABF0D63/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Architekt/7FA1A118-D0EA-445C-AFA9-BFA05F3A36BF/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Applikationsingenieur/E9BF9DE1-89BA-42FA-8DB3-70C4E43D1443/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Cloud-Engineer/4F70CA09-E2E8-4AAC-9B54-5F900246F844/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/IT-Berater/B0C4E890-8BCF-4D2A-A5FB-63B26B5C4150/j/Contracting/3/p/1?q=&e=false&pt=false",
        "https://www.hays.de/jobsuche/stellenangebote-jobs/s/IT/1/r/Systemingenieur/0793F3E3-0C39-40DE-8B65-9962666F635E/j/Contracting/3/p/1?q=&e=false&pt=false"
    ];

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
        var (numberOfEntries, mainPage) = await ScrapeNumberOfProjects(spezialisierungUrl);
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / 20);

        var scrapePageTasks = new List<Task<List<Project>>>();
        for (var page = 0; page < numberOfPages; page++)
        {
            scrapePageTasks.Add(ScrapeSearchPage(mainPage, page));
        }
        Task.WaitAll([.. scrapePageTasks]);
        return scrapePageTasks.SelectMany(t => t.Result).ToList();
    }

    private async Task<List<Project>> ScrapeSearchPage(HtmlDocument mainPage, int page)
    {
        var projectUrlsFromPage = (page == 0) ? ExtractProjectUrls(mainPage) : await ScrapeProjectUrlsFromSearchSite(page);
        return await ScrapeProjectsByUrl(projectUrlsFromPage);
    }

    private async Task<List<Project>> ScrapeProjectsByUrl(string[] projectUrls)
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

    private async Task<string[]> ScrapeProjectUrlsFromSearchSite(string spezialisierungUrl, int page = 0, int retry = 0)
    {
        try
        {
            var url = $"{spezialisierungUrl}/j/Contracting/3/p/{page + 1}?q=&e=false&pt=false";
            var document = await _httpHelper.GetHtml(url);
            return ExtractProjectUrls(document);
        }
        catch
        {
            if (retry > 3) throw;
            return await ScrapeProjectUrlsFromSearchSite(spezialisierungUrl, page, retry + 1);
        }
    }

    private static string[] ExtractProjectUrls(HtmlDocument searchSite)
    {
        var projectUrls = searchSite.DocumentNode.SelectNodes("//a[@class='search__result__header__a']");
        return projectUrls.Select(url => url.GetAttributeValue("href", "")).Where(s => s != "").ToArray();
    }

    private async Task<(int, HtmlDocument)> ScrapeNumberOfProjects(string categoryUrl, int retry = 0)
    {
        try
        {
            var mainPage = await _httpHelper.GetHtml(categoryUrl);
            var numberOfEntriesDiv = mainPage.DocumentNode.SelectSingleNode("//div[@class='hays__search__number-of-results']");
            var innerText = numberOfEntriesDiv.InnerText.Trim();
            innerText = innerText.Replace("\n", "").Replace(" ", "").Replace("Ergebnisse", "").Replace("Ergebnis", "");
            return (int.Parse(innerText), mainPage);
        }
        catch
        {
            if (retry > 3) throw;
            return await ScrapeNumberOfProjects(categoryUrl, retry + 1);
        }
    }

    private async Task<Project?> ScrapeProject(string projectUrl, int retry = 0)
    {
        try
        {
            _logger.LogInformation($"Scraping project, retry: {retry}, url {projectUrl}");
            var projectSite = await _httpHelper.GetHtml(projectUrl);

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

            var descriptionString = description.GetString() ?? "".Replace("Der Bereich IT ist unsere Kernkompetenz, auf deren Grundlage sich Hays entwickelt hat. Wir sind das gr��te privatwirtschaftlich organisierte IT-Personaldienstleistungsunternehmen in Deutschland und haben f�r jede Karrierestufe das passende Angebot � egal ob Sie an Vakanzen in agilen KMUs oder starken DAX-Konzernen interessiert sind. Wir beherrschen die komplette IT-Klaviatur von Support bis zur Softwarearchitektur oder Digitalisierung � dank unseres umfangreichen Portfolios ist f�r jeden etwas dabei. So konnten wir in den vergangenen Jahrzehnten im Rahmen einer Life-Long-Partnerschaft unz�hlige Fach- und F�hrungskr�fte aus der IT dabei unterst�tzen, die Weichen f�r eine erfolgreiche Karriere zu stellen. Unser Beratungsteam ist spezialisiert und somit in der Lage, auf Ihre W�nsche und Vorstellungen einzugehen und Sie auf Bewerbungsgespr�che und Vertragsverhandlungen bestens vorzubereiten. Probieren Sie es aus und erfahren Sie, was der Markt Ihnen zu bieten hat � v�llig kostenfrei, diskret und unverbindlich! Wir freuen uns auf Sie.", "");
            var jobLocationString = $"{jobLocationType.GetString() ?? ""}, {jobLocationAddressCountry.GetString() ?? ""}, {jobLocationAddressPostalCode.GetString() ?? ""}, {jobLocationAddressLocality.GetString() ?? ""}";

            var project = new Project(
                source: ProjectSource.Hays,
                title: title,
                url: url,
                projectIdentifier: identifier.GetString() ?? "",
                description: descriptionString,
                jobLocation: jobLocationString
            );

            return project;
        }
        catch
        {
            _logger.LogInformation($"Error scraping project, retry: {retry}, url {projectUrl}");
            if (retry > 2) return null;
            return await ScrapeProject(projectUrl, retry + 1);
        }
    }
}
