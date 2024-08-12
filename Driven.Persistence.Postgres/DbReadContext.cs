using Application.Ports;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres;

public class DbReadContext : IReadContext
{
    private readonly Context _context;

    public DbReadContext(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IQueryable<Project> Projects => _context.Projects;

    public async Task<List<T>> ToListAsync<T>(IQueryable<T> query) where T : class
    {
        return await query.ToListAsync();
    }
}