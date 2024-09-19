namespace Domain;

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

    public void ReTag(Tag[] tags)
    {
        Tags.Clear();
        EvaluateAndAddTags(tags);
    }

    public void EvaluateAndAddTags(Tag[] tags)
    {
        var existingTags = Tags.Select(t => t.Id).ToArray();
        foreach (var tag in tags)
        {
            if (existingTags.Contains(tag.Id)) continue;
            if (tag.IsApplicable(Title) || tag.IsApplicable(Description) || tag.IsApplicable(Url))
            {
                Tags.Add(tag);
            }
        }
    }

    public bool IsSameProject(Project other)
    {
        if (Source != other.Source) return false;
        if (ProjectIdentifier != other.ProjectIdentifier) return false;
        if (Url != other.Url) return false;
        if (Title != other.Title) return false;

        return true;
    }

    public string ToLogMessage() => $"{Source}; {PostedAt}; ({Url})";
}
