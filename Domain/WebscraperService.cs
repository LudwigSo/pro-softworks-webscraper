namespace Domain;

public interface IWebscraperPort
{
    Task<List<Project>> Scrape(ProjectSource source);
}

public interface IProjectRepository
{
    Task AddRange(List<Project> projects);
}

public class WebscraperService
{
    private readonly IWebscraperPort _port;
    private readonly IProjectRepository _projectRepository;

    public WebscraperService(IWebscraperPort port, IProjectRepository projectRepository)
    {
        _port = port ?? throw new ArgumentNullException(nameof(port));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    public async Task Scrape()
    {
        var projects = await _port.Scrape(ProjectSource.Hays);
        await _projectRepository.AddRange(projects);
    }
}


public class Project
{
    public Project(ProjectSource source, string title, string url, string? projectIdentifier, string? description, string? jobLocation)
    {
        Source = source;
        Title = title;
        Url = url;
        ProjectIdentifier = projectIdentifier;
        Description = description;
        JobLocation = jobLocation;
    }

    public int Id { get; init; }
    public ProjectSource Source { get; }
    public string Title { get; }
    public string Url { get; }
    public string? ProjectIdentifier { get; }
    public string? Description { get; }
    public string? JobLocation { get; }

    public bool IsCSharp =>
        Description?.Contains("C#") ?? false
        || Title.Contains("C#");

    public bool IsDotNet =>
        Description?.Contains(".NET") ?? false
        || Title.Contains(".NET");
}

public enum ProjectSource
{
    Hays = 0,
}