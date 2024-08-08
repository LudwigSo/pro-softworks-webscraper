namespace Domain;

public interface ICrudUnitOfWork
{
    Task<T?> Find<T>(object[] primaryKey) where T : class;
    Task AddRange<T>(IEnumerable<T> projects) where T : class;
    Task Add<T>(T project) where T : class;
    void Remove<T>(T project) where T : class;
    Task<int> SaveChangesAsync();
}
