using Domain.Model;

namespace Domain.Services.Webscraper;

public interface IProjectQueries
{
    Task<List<Project>> GetActiveBySource(ProjectSource source);
}
