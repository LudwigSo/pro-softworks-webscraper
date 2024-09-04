using Domain.Ports;
using Domain.Model;
using FakeItEasy;
using Domain.CommandHandlers;

namespace Domain.Test;

public class ScrapeAndProcessCommandHandlerTests
{


    [Fact]
    public async Task Test1()
    {
        // Arrange
        var scrapedProjects = new List<Project>
        {
            DomainFactory.NewProject(title: "Project 1", projectIdentifier: "123"),
            DomainFactory.NewProject(title: "Project 2", projectIdentifier: "234"),
            DomainFactory.NewProject(title: "Project 3", projectIdentifier: "345"),
        };

        var fakeLogger = A.Fake<ILogger>();
        var fakeSignalR = A.Fake<IRealtimeMessagesPort>();
        var fakeWriteContext = A.Fake<IWriteContext>();
        var fakeProjectQueries = A.Fake<IProjectQueriesPort>();
        A.CallTo(() => fakeProjectQueries.GetActiveBySource(ProjectSource.Hays)).Returns([]);

        var fakeWebscraperPort = A.Fake<IWebscraperPort>();
        A.CallTo(() => fakeWebscraperPort.Scrape(ProjectSource.Hays)).Returns(scrapedProjects);

        var handler = new ScrapeAndProcessCommandHandler(fakeWebscraperPort, fakeProjectQueries, fakeWriteContext, fakeSignalR, fakeLogger);
        var command = new ScrapeAndProcessCommand(ProjectSource.Hays);

        // Act
        await handler.Handle(command);

        // Assert
        A.CallTo(() => fakeWebscraperPort.Scrape(ProjectSource.Hays)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeWriteContext.AddRange(scrapedProjects)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeWriteContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeSignalR.NewProjectsAdded(scrapedProjects)).MustHaveHappenedOnceExactly();
    }
}