using Domain.Model;

namespace Domain.Ports
{
    public interface IProjectQueriesPort
    {
        Task<Project[]> GetActiveBySource(ProjectSource source);
        Task<Project[]> GetActive();
    }
}
