using Application.Ports;
using Application.Webscraper;
using Domain.Model;
using Domain.Ports;
using FakeItEasy;

namespace Application.Test;

public class ScrapeAndProcessCommandHandlerTests
{
    

    [Fact]
    public async Task Test1()
    {
        // Arrange
        var projects = new List<Project>
        {
            DomainFactory.NewProject(title: "Project 1", projectIdentifier: "123"),
            DomainFactory.NewProject(title: "Project 2", projectIdentifier: "234"),
            DomainFactory.NewProject(title: "Project 3", projectIdentifier: "345"),
        };

        var fakeLogger = A.Fake<ILogger>();
        var fakeWriteContext = A.Fake<IWriteContext>();
        var fakeReadContext = FakeFactory.NewReadContext();

        var fakeWebscraperPort = A.Fake<IWebscraperPort>();
        A.CallTo(() => fakeWebscraperPort.Scrape(ProjectSource.Hays)).Returns(projects);

        var handler = new ScrapeAndProcessCommandHandler(fakeWebscraperPort, fakeReadContext, fakeWriteContext, fakeLogger);
        var command = new ScrapeAndProcessCommand(ProjectSource.Hays);

        // Act
        await handler.Handle(command);

        // Assert
        A.CallTo(() => fakeWebscraperPort.Scrape(ProjectSource.Hays)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeWriteContext.AddRange(projects)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeWriteContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
    }
}