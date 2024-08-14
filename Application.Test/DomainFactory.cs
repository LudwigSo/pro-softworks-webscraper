using Domain.Model;

namespace Application.Test;

public static class DomainFactory
{
    public static Project NewProject(
        ProjectSource source = ProjectSource.Hays,
        string title = "Project 1",
        string url = "http://example.com",
        string? projectIdentifier = "123",
        string? description = "Description",
        string? jobLocation = "London"
    )
    {
        return new Project(
            source,
            title,
            url,
            projectIdentifier,
            description,
            jobLocation
        );
    }
}