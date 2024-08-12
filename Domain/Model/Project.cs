namespace Domain.Model;

public enum ProjectSource
{
    Hays = 0,
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
        FirstSeenAt = DateTime.Now;
        IsActive = true;
    }

    public int Id { get; init; }
    public ProjectSource Source { get; }
    public string Title { get; }
    public string Url { get; }
    public string? ProjectIdentifier { get; }
    public string? Description { get; }
    public string? JobLocation { get; }
    public DateTime FirstSeenAt { get; }
    public DateTime? RemovedAt { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsCSharp =>
        Description?.Contains("C#") ?? false
        || Title.Contains("C#");

    public bool IsDotNet =>
        Description?.Contains(".NET") ?? false
        || Title.Contains(".NET");

    public void MarkAsRemoved()
    {
        if (RemovedAt.HasValue && IsActive == false) throw new InvalidOperationException("Project is already removed");
        RemovedAt = DateTime.Now;
        IsActive = false;
    }

    public bool IsSameProject(Project other)
    {
        if (Source != other.Source) return false;
        if (ProjectIdentifier != other.ProjectIdentifier) return false;
        if (Url != other.Url) return false;
        if (Title != other.Title) return false;

        return true;
    }
}
