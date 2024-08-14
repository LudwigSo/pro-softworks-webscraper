using Application.Ports;
using Application.Webscraper;
using Domain.Model;
using FakeItEasy;

namespace Application.Test;

public class ScrapeAndProcessCommandHandlerTests
{
    private static Project ProjectFactory(
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

    [Fact]
    public async Task Test1()
    {
        // Arrange
        var projects = new List<Project>
        {
            ProjectFactory(title: "Project 1", projectIdentifier: "123"),
            ProjectFactory(title: "Project 2", projectIdentifier: "234"),
            ProjectFactory(title: "Project 3", projectIdentifier: "345"),
        };

        var fakeWebscraperPort = A.Fake<IWebscraperPort>();
        A.CallTo(() => fakeWebscraperPort.Scrape(ProjectSource.Hays)).Returns(projects);

        var fakeReadContext = A.Fake<IReadContext>();
        A.CallTo(() => fakeReadContext.Projects).Returns(new List<Project>().AsQueryable());

        var fakeWriteContext = A.Fake<IWriteContext>();

        var handler = new ScrapeAndProcessCommandHandler(fakeWebscraperPort, fakeReadContext, fakeWriteContext);
        var command = new ScrapeAndProcessCommand(ProjectSource.Hays);

        // Act
        await handler.Handle(command);

        // Assert
        A.CallTo(() => fakeWebscraperPort.Scrape(ProjectSource.Hays)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeWriteContext.AddRange(projects)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeWriteContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }
}