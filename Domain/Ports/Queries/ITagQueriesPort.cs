using Domain.Model;

namespace Domain.Ports.Queries
{
    public interface ITagQueriesPort
    {
        Task<Tag[]> GetAllTags();
        Task<Tag?> GetTag(int id);
    }
}
