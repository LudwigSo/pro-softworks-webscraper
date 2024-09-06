using Domain.Model;

namespace Domain.Ports.Queries
{
    public interface IProjectQueriesPort
    {
        Task<Project[]> GetActiveBySource(ProjectSource source);
        Task<Project[]> GetActiveWithAnyTag();
        Task<Project[]> GetAll(int page = 0, int skipPerPage = 1000, int take = 1000);
        Task<int> GetProjectCount();
    }
}
