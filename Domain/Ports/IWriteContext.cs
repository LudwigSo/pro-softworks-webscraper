namespace Domain.Ports;

public interface IWriteContext
{
    Task AddRange<T>(IEnumerable<T> entity) where T : class;
    void Remove<T>(T entity) where T : class;
    Task<int> SaveChangesAsync();
}
