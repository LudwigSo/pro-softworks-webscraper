namespace Domain.Model;

public class Project
{
    public int Id { get; init; }
    public ProjectSource Source { get; }
    public string Title { get; }
    public string Url { get; }
    public string? ProjectIdentifier { get; }
    public string? Description { get; }
    public string? JobLocation { get; }
    public DateTime? PlannedStart { get; }
    public DateTime? PostedAt { get; }
    public DateTime FirstSeenAt { get; } = DateTime.Now;
    public DateTime? RemovedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public List<Tag> Tags { get; } = new();

    public Project(
        ProjectSource source,
        string title,
        string url,
        string? projectIdentifier = null,
        string? description = null,
        string? jobLocation = null,
        DateTime? plannedStart = null,
        DateTime? postedAt = null
    )
    {
        Source = source;
        Title = title;
        Url = url;
        ProjectIdentifier = projectIdentifier;
        Description = description;
        JobLocation = jobLocation;
        PlannedStart = plannedStart;
        PostedAt = postedAt;
    }

    internal void ReTag(Tag[] tags)
    {
        Tags.Clear();
        EvaluateAndAddTags(tags);
    }

    internal void EvaluateAndAddTags(Tag[] tags)
    {
        var existingTags = Tags.Select(t => t.Id).ToArray();
        foreach (var tag in tags)
        {
            if (existingTags.Contains(tag.Id)) continue;
            if (TagIsApplicable(tag))
            {
                Tags.Add(tag);
            }
        }
    }
    
    private bool TagIsApplicable(Tag tag)
    {
        if (Title.Contains(tag.Name, StringComparison.OrdinalIgnoreCase)) return true;
        if (Description?.Contains(tag.Name, StringComparison.OrdinalIgnoreCase) ?? false) return true;
        return false;
    }

    internal void MarkAsRemoved()
    {
        if (RemovedAt.HasValue && IsActive == false) throw new InvalidOperationException("Project is already removed");
        RemovedAt = DateTime.Now;
        IsActive = false;
    }

    internal bool IsSameProject(Project other)
    {
        if (Source != other.Source) return false;
        if (ProjectIdentifier != other.ProjectIdentifier) return false;
        if (Url != other.Url) return false;
        if (Title != other.Title) return false;

        return true;
    }
}
