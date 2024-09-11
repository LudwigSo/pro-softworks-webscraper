using Domain.Model;

namespace Domain.Test;

public static class DomainFactory
{
    public static Project NewProject(
        ProjectSource source = ProjectSource.Hays,
        string title = "Project 1",
        string url = "http://example.com",
        string? projectIdentifier = "123",
        string? description = "Description",
        string? jobLocation = "London",
        DateTime? plannedStart = null,
        DateTime? postedAt = null
    )
    {
        plannedStart ??= new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Local);
        postedAt ??= new DateTime(2024, 10, 14, 9, 0, 32, DateTimeKind.Local);

        return new Project(
            source,
            title,
            url,
            projectIdentifier,
            description,
            jobLocation,
            plannedStart,
            postedAt
        );
    }
}