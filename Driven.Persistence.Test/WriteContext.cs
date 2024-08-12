using Application.Ports;

namespace Driven.Persistence.Postgres;

public class WriteContext : IWriteContext
{
    private readonly Context _context;

    public WriteContext(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<T?> Find<T>(object[] primaryKey) where T : class
    {
        return await _context.FindAsync<T>(primaryKey);
    }

    public async Task AddRange<T>(IEnumerable<T> projects) where T : class
    {
        await _context.AddRangeAsync(projects);
    }

    public async Task Add<T>(T project) where T : class
    {
        await _context.AddAsync(project);
    }

    public void Remove<T>(T project) where T : class
    {
        _context.Remove(project);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
