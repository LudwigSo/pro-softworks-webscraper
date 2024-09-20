using System.Text.Json;
using Domain;
using Application.Ports;
using Driven.Webscraper.Proxy;
using HtmlAgilityPack;

namespace Driven.Webscraper.Scraper;

public class HaysWebscraper(ILogging logger, HttpHelper httpHelper) : AbstractCommonWebscraper
{
    private readonly ILogging _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpHelper _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    private readonly ProjectSource _projectSource = ProjectSource.Hays;
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
            var projectsForSpezialisierung = await ScrapeSearchSiteParallel(spezialisierungUrl);
            projects.AddRange(projectsForSpezialisierung);
        }

        return projects;
    }

    protected async Task<List<Project>> ScrapeSearchSiteParallel(string url, int projectsPerPage = 20)
    {
        var numberOfEntries = await ScrapeNumberOfProjects(url);
        var numberOfPages = (int)Math.Ceiling((double)numberOfEntries / projectsPerPage);

        var scrapePageTasks = new List<Task<List<Project>>>();
        for (var page = 0; page < numberOfPages; page++)
        {
            var task = ScrapeSearchPage(url, page);
            scrapePageTasks.Add(task);
        }
        Task.WaitAll([.. scrapePageTasks]);
        return scrapePageTasks.SelectMany(t => t.Result).ToList();
    }

    protected virtual async Task<List<Project>> ScrapeSearchPage(string url, int page)
    {
        var projectUrlsFromPage = await ScrapeProjectUrlsFromSearchSite(url, page);
        return await ScrapeProjectsByUrl(projectUrlsFromPage);
    }

    protected async Task<string[]> ScrapeProjectUrlsFromSearchSite(string spezialisierungUrl, int page = 0, int retry = 0)
    {
        try
        {
            var url = $"{spezialisierungUrl}/j/Contracting/3/p/{page + 1}?q=&e=false&pt=false";
            var document = await _httpHelper.GetHtml(url, withProxy: false);
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

    protected async Task<int> ScrapeNumberOfProjects(string categoryUrl, int retry = 0)
    {
        try
        {
            var mainPage = await _httpHelper.GetHtml(categoryUrl, withProxy: false);
            var numberOfEntriesDiv = mainPage.DocumentNode.SelectSingleNode("//div[@class='hays__search__number-of-results']");
            var innerText = numberOfEntriesDiv.InnerText.Trim();
            innerText = innerText.Replace("\n", "").Replace(" ", "").Replace("Ergebnisse", "").Replace("Ergebnis", "");
            var amountOfProjects = int.Parse(innerText);
            _logger.LogInformation($"{_projectSource}: {amountOfProjects} projects found, retry: {retry}, url {categoryUrl}");
            return amountOfProjects;
        }
        catch
        {
            if (retry > 3) throw;
            return await ScrapeNumberOfProjects(categoryUrl, retry + 1);
        }
    }

    protected override async Task<Project?> ScrapeProject(string projectUrl, int retry = 0)
    {
        try
        {
            _logger.LogDebug($"{_projectSource}: Start to scrape project, retry: {retry}, url {projectUrl}");
            var projectSite = await _httpHelper.GetHtml(projectUrl, withProxy: false);

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

            var descriptionString = description.GetString() ?? "".Replace("Der Bereich IT ist unsere Kernkompetenz, auf deren Grundlage sich Hays entwickelt hat. Wir sind das größte privatwirtschaftlich organisierte IT-Personaldienstleistungsunternehmen in Deutschland und haben für jede Karrierestufe das passende Angebot – egal ob Sie an Vakanzen in agilen KMUs oder starken DAX-Konzernen interessiert sind. Wir beherrschen die komplette IT-Klaviatur von Support bis zur Softwarearchitektur oder Digitalisierung – dank unseres umfangreichen Portfolios ist für jeden etwas dabei. So konnten wir in den vergangenen Jahrzehnten im Rahmen einer Life-Long-Partnerschaft unzählige Fach- und Führungskräfte aus der IT dabei unterstützen, die Weichen für eine erfolgreiche Karriere zu stellen. Unser Beratungsteam ist spezialisiert und somit in der Lage, auf Ihre Wünsche und Vorstellungen einzugehen und Sie auf Bewerbungsgespräche und Vertragsverhandlungen bestens vorzubereiten. Probieren Sie es aus und erfahren Sie, was der Markt Ihnen zu bieten hat – völlig kostenfrei, diskret und unverbindlich! Wir freuen uns auf Sie.", "");
            var jobLocationString = $"{jobLocationType.GetString() ?? ""}, {jobLocationAddressCountry.GetString() ?? ""}, {jobLocationAddressPostalCode.GetString() ?? ""}, {jobLocationAddressLocality.GetString() ?? ""}";

            var project = new Project(
                source: ProjectSource.Hays,
                title: title,
                url: url,
                projectIdentifier: identifier.GetString() ?? "",
                description: descriptionString,
                jobLocation: jobLocationString
            );

            _logger.LogDebug($"Succeeded scraping project ({retry}): {project.ToLogMessage()}");
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
