using Domain.Model;

namespace Domain.Ports
{
    public interface IProjectQueriesPort
    {
        Task<Project[]> GetActiveBySource(ProjectSource source);
        Task<Project[]> GetActive();
        Task<Project[]> GetAll(int page = 0, int skipPerPage = 1000, int take = 1000);
        Task<int> GetProjectCount();
        Task<Tag[]> GetAllTags();
    }
}
