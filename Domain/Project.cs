namespace Domain;

public class Project
{
    public int Id { get; init; }
    public ProjectSource Source { get; }
    public string Title { get; } = "";
    public string Url { get; } = "";
    public string? ProjectIdentifier { get; }
    public string? Description { get; }
    public string? JobLocation { get; }
    public string? PlannedStartAsString { get; }
    public DateTime? PlannedStart { get; }
    public DateTime? PostedAt { get; }
    public DateTime FirstSeenAt { get; } = DateTime.Now;
    public List<Tag> Tags { get; } = new();

    protected Project() { }

    public Project(
        ProjectSource source,
        string title,
        string url,
        string? projectIdentifier = null,
        string? description = null,
        string? jobLocation = null,
        string? plannedStartAsString = null,
        DateTime? plannedStart = null,
        DateTime? postedAt = null
    )
    {
        Source = source;
        Title = LimitString(title, 500);
        Url = url;
        ProjectIdentifier = LimitString(projectIdentifier, 200);
        Description = LimitString(description, 5000);
        JobLocation = LimitString(jobLocation, 200);
        PlannedStartAsString = LimitString(plannedStartAsString, 250);
        PlannedStart = plannedStart;
        PostedAt = postedAt;
    }

    private string LimitString(string? input, int length)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Substring(0, Math.Min(input.Length, length));
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
