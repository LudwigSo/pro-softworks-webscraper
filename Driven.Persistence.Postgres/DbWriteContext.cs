using Domain.Ports;

namespace Driven.Persistence.Postgres;

public class DbWriteContext(Context context) : IWriteContext
{
    private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));

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
