using Domain.Model;
using Domain.Services.Webscraper;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres.Queries;

public class ProjectQueries(Context context) : IProjectQueries
{
    private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<Project>> GetActiveBySource(ProjectSource source)
    {
        return await _context.Projects.Where(p => p.Source == source && p.RemovedAt == null).ToListAsync();
    }
}
