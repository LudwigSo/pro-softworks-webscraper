namespace Domain.Model;

public enum ProjectSource
{
    Hays = 0,
}

public class Project(
    ProjectSource source,
    string title,
    string url,
    string? projectIdentifier,
    string? description,
    string? jobLocation)
{
    public int Id { get; init; }
    public ProjectSource Source { get; } = source;
    public string Title { get; } = title;
    public string Url { get; } = url;
    public string? ProjectIdentifier { get; } = projectIdentifier;
    public string? Description { get; } = description;
    public string? JobLocation { get; } = jobLocation;
    public DateTime FirstSeenAt { get; } = DateTime.Now;
    public DateTime? RemovedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

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
