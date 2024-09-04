using Domain.Model;

namespace Domain.Ports
{
    public interface IProjectQueriesPort
    {
        Task<List<Project>> GetActiveBySource(ProjectSource source);
    }
}
